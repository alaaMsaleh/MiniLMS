# Mini LMS

Backend for a small learning management system, built for the Keyformance take-home assignment. An admin writes questions and puts them into quizzes. Students take a published quiz, submit their answers and get a score back. The admin can look at how each student is doing.

Stack: ASP.NET Core (.NET 8), Entity Framework Core, SQL Server, ASP.NET Identity with JWT bearer tokens, Swagger.

## Running it

You need the .NET 8 SDK and a SQL Server instance.

**1. Clone and restore**

```
git clone https://github.com/alaaMsaleh/MiniLMS.git
cd MiniLMS
dotnet restore
```

**2. JWT key**

Nothing to do here. `appsettings.json` contains a development-only signing key, so the project runs right after cloning. It is only meant for local testing. If you want to use your own, override it with user secrets or an environment variable (at least 32 characters, the app will not start with a shorter one):

```
dotnet user-secrets init --project src/MiniLMS.API
dotnet user-secrets set "JWT:AuthKey" "your-own-long-random-string-of-32-or-more-chars" --project src/MiniLMS.API
```

or set `JWT__AuthKey` as an environment variable.

**3. Database**

`appsettings.json` points at `(localdb)\MSSQLLocalDB`, so on Windows with Visual Studio or SQL Server Express LocalDB there is nothing to configure.

On Mac/Linux, or if you prefer a normal SQL Server, start one (for example with Docker) and override the connection string:

```
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Your_password123" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

dotnet user-secrets init --project src/MiniLMS.API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=MiniLMSDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True" --project src/MiniLMS.API
```

You do not need to run `dotnet ef`. On startup the app applies the migrations and seeds the data (see below).

**4. Run**

```
dotnet run --project src/MiniLMS.API --launch-profile https
```

Swagger is at `https://localhost:7056/swagger`.

## Seeded accounts

| Role    | Email                  | Password      |
|---------|------------------------|---------------|
| Admin   | admin@minilms.com      | Admin@12345   |
| Student | student@minilms.com    | Student@12345 |
| Student | student2@minilms.com   | Student@12345 |

The seed also creates four sample questions and one published quiz, "General Knowledge Quiz". The seeding only inserts what is missing, so restarting the app does not duplicate anything.

## Trying the main flow in Swagger

1. `POST /api/Auth/login` as the admin, copy the token, click Authorize and paste it (just the token, no "Bearer").
2. Create a question with `POST /api/Questions`, then a quiz with `POST /api/quizzes` using the question ids.
3. New quizzes start unpublished. Call `POST /api/quizzes/{id}/publish`.
4. Log in as `student@minilms.com` and authorize with that token instead.
5. `GET /api/student/quizzes` to see what is available, `GET /api/student/quizzes/{id}` for the questions, then `POST /api/student/quizzes/{id}/submit` with the answers.
6. `GET /api/student/results` for the history.
7. Switch back to the admin token and call `GET /api/admin/performance/students` or `GET /api/admin/performance`.

A submit body looks like this. A question you leave out, or send with `selectedChoiceId: null`, counts as unanswered.

```json
{
  "answers": [
    { "questionId": 1, "selectedChoiceId": 3 },
    { "questionId": 2, "selectedChoiceId": null }
  ]
}
```

## Endpoints

| Method | Route | Who | What |
|--------|-------|-----|------|
| POST | /api/Auth/login | anyone | returns a JWT |
| POST | /api/Questions | Admin | create a question with its choices |
| GET | /api/Questions, /api/Questions/{id} | Admin | list / get |
| PUT | /api/Questions/{id} | Admin | edit text, image and choices |
| DELETE | /api/Questions/{id} | Admin | remove (soft delete) |
| POST | /api/quizzes | Admin | create a quiz from question ids |
| GET | /api/quizzes, /api/quizzes/{id} | Admin | list / get |
| POST | /api/quizzes/{id}/publish, /unpublish | Admin | control what students can see |
| DELETE | /api/quizzes/{id} | Admin | remove (soft delete) |
| GET | /api/admin/performance | Admin | all attempts, optional `quizId` and `studentId` filters |
| GET | /api/admin/performance/students | Admin | one summary row per student |
| GET | /api/admin/performance/submissions/{id} | Admin | one attempt in detail |
| GET | /api/student/quizzes, /{id} | Student | published quizzes (no correct answers in the response) |
| POST | /api/student/quizzes/{id}/submit | Student | submit answers, get the result |
| GET | /api/student/results, /{submissionId} | Student | my own attempts only |

Roles are checked with `[Authorize(Roles = ...)]` on each controller. The student id always comes from the token, never from the request, so a student cannot ask for someone else's results (asking for another student's submission id returns 404).

## Project layout

```
src/
  MiniLMS.Domain          entities
  MiniLMS.Application     DTOs, service interfaces, exceptions
  MiniLMS.Infrastructure  DbContext, EF configuration, migrations, services, seeder
  MiniLMS.API             controllers, middleware, Program.cs
```

The API depends on the other projects, Infrastructure depends on Application and Domain. I kept it to these four on purpose. Question has a small repository because the update needs a transaction; the quiz services use the DbContext directly since they are mostly queries, and I did not want a repository that only forwards calls.

## Data model

- `Question`: text, optional image URL, `IsDeleted`
- `Choice`: text, `IsCorrect`, `IsDeleted`
- `Quiz`: title, description, duration, `IsPublished`, `IsDeleted`
- `QuizQuestion`: join table between quiz and question, with an `Order`
- `QuizSubmission`: one attempt (student, quiz, attempt number, score, correct/incorrect counts, status)
- `StudentAnswer`: one row per question in an attempt
- `User`: Identity user with `FullName`

A filtered unique index on `Choice` makes SQL Server itself enforce "one correct choice per question", not only the validation code. The choice limit of 2 to 6 and the single correct answer are also checked in the service, so the user gets a clear message before reaching the database.

## Decisions

**Multiple quizzes.** I went with quizzes the admin builds from existing questions instead of one quiz with everything. It costs a join table and a publish step, but it is closer to how a school would use it (a quiz per topic) and a draft quiz can be prepared without students seeing it. Students only see published quizzes.

**What happens to results when a question changes.** This was the part I spent the most time on. When a student submits, each `StudentAnswer` stores the question text, the text of the selected choice and the text of the correct choice, along with whether it was right. Results are always shown from that stored copy. So if the admin later rewrites a question or changes the correct answer, an old result still shows exactly what the student saw and the score does not move.

**Editing a question.** The question keeps its id, so quizzes that contain it keep working. Its old choices are marked deleted and the new ones are inserted, all inside one transaction. I replaced choices instead of editing them one by one because it is simpler and the stored copies make old results safe either way. The downside is that choices get new ids on every edit. Students see the new version of the question straight away, even in a quiz that is already published.

**Removing content.** Deleting a question or a quiz only sets `IsDeleted`; nothing is physically removed. Global query filters hide deleted rows everywhere, except when reading past results, which ignore the filters on purpose. So a deleted question disappears from quizzes and from the question count, but attempts that already included it still show it. Deleting a quiz also unpublishes it. A quiz cannot be published if it has no active questions.

**Retakes.** A student can take a quiz as many times as they like. Every submission is kept as its own attempt with an attempt number (1, 2, 3...), nothing is overwritten. The student sees all of them, the admin sees all of them, and the students overview shows average and best percentage. I chose this over "one attempt only" because it loses no data, and it is easy to restrict later by checking the attempt count at submit time.

**Scoring.** One point per correct answer, so the score equals the number of correct answers. Unanswered questions count as incorrect. The submit is rejected if the same question appears twice, if a question is not part of the quiz, or if the chosen choice does not belong to that question.

**Double submits.** Attempts have a unique index on quiz, student and attempt number, and the submission has a row version column. See the limitations below for what that does and does not cover.

**Errors and logging.** One middleware catches exceptions and returns `ProblemDetails`: 400 for invalid input, 404 for missing things, 409 for conflicts, and a generic 500 message that never includes the exception text. Model validation errors from the DTO attributes come back as 400 in the standard format. Services log the important events (question and quiz changes, submissions with student, quiz, attempt and score), failed logins are logged as warnings, and unexpected errors are logged with the stack trace and a trace id that is also in the response.

## Assumptions

- Accounts come from the seeder, so there is no registration endpoint.
- The JWT key in `appsettings.json` is a development key that I left in the repo on purpose so the project runs without any setup. In a real deployment it would come from a secret store or environment variable, and the same applies to the connection string.
- Question images are just URLs, validated as http or https. No upload.
- The quiz duration is stored and shown to the student but not enforced. There is no timer on the server, the submit is accepted whenever it arrives.
- Admins edit questions, not quizzes. Once a quiz is created, its list of questions can't be changed; you create a new quiz or edit the questions inside it.

## Known limitations and what I would do with more time

- Automated tests. This is the main gap. I would start with the scoring rules, the update/delete behaviour of questions, and the submit validation.
- If one student sends two submits for the same quiz at exactly the same moment, both can calculate the same attempt number. The unique index stops the second one, but right now that surfaces as a 500 instead of a clean 409. I would catch the `DbUpdateException` and return a conflict.
- Quiz editing (add or remove questions) and server-side time limits.
- Pagination on the list endpoints, which matters for the admin performance list once there are many attempts.
- A docker-compose file for the API and the database, and refresh tokens (the access token currently lasts 30 days, set by `JWT:DurationInDays`).