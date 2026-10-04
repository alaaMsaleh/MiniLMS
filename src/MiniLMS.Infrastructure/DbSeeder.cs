using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniLMS.Domain.Entities;
using MiniLMS.Infrastructure.DBContext;

namespace MiniLMS.Infrastructure
{
    public static class DbSeeder
    {
        public const string AdminRole = "Admin";
        public const string StudentRole = "Student";

        public static async Task SeedAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

            await context.Database.MigrateAsync();

            // Roles
            foreach (var role in new[] { AdminRole, StudentRole })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
            }

            // Users (demo credentials, documented in README)
            await CreateUserAsync(userManager, "admin@minilms.com", "Admin User", "Admin@12345", AdminRole);
            await CreateUserAsync(userManager, "student@minilms.com", "Student User", "Student@12345", StudentRole);
            await CreateUserAsync(userManager, "student2@minilms.com", "Second Student", "Student@12345", StudentRole);

            // Questions + Quiz
            if (!await context.Questions.IgnoreQueryFilters().AnyAsync())
            {
                var questions = new List<Question>
                {
                    NewQuestion("What is the capital of Egypt?", "Cairo", "Alexandria", "Aswan"),
                    NewQuestion("Which keyword makes a class inherit from another in C#?", ":", "extends", "inherits"),
                    NewQuestion("Which HTTP status code means 'Not Found'?", "404", "200", "500", "401"),
                    NewQuestion("Which EF Core command creates a migration?", "dotnet ef migrations add", "dotnet ef create", "dotnet migrate"),
                };

                context.Questions.AddRange(questions);
                await context.SaveChangesAsync();

                var quiz = new Quiz
                {
                    Title = "General Knowledge Quiz",
                    Description = "A short sample quiz",
                    DurationInMinutes = 15,
                    IsPublished = true,
                    QuizQuestions = questions
                        .Select((q, i) => new QuizQuestion { Question = q, Order = i + 1 })
                        .ToList()
                };

                context.Quizzes.Add(quiz);
                await context.SaveChangesAsync();
            }
        }


        private static Question NewQuestion(string text, string correct, params string[] wrong)
        {
            var question = new Question { Text = text };
            question.Choices.Add(new Choice { Text = correct, IsCorrect = true });
            foreach (var w in wrong)
                question.Choices.Add(new Choice { Text = w, IsCorrect = false });
            return question;
        }

        private static async Task CreateUserAsync(
            UserManager<User> userManager, string email, string fullName, string password, string role)
        {
            if (await userManager.FindByEmailAsync(email) != null) return;

            var user = new User
            {
                UserName = email,
                Email = email,

                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    "Seeding user failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));

            await userManager.AddToRoleAsync(user, role);
        }
    }
}
