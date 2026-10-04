using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniLMS.Application.DTOs;
using MiniLMS.Application.DTOs.QuizzesDtos;
using MiniLMS.Application.DTOs.StudentDto;
using MiniLMS.Application.Interfaces;
using MiniLMS.Domain.Entities;
using MiniLMS.Infrastructure.DBContext;

namespace MiniLMS.Infrastructure.Services
{
    public class StudentQuizService : IStudentQuizService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StudentQuizService> _logger;

        public StudentQuizService(ApplicationDbContext context, ILogger<StudentQuizService> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<List<StudentQuizSummaryDto>> GetAvailableQuizzesAsync() =>
            await _context.Quizzes
                .AsNoTracking()
                .Where(q => q.IsPublished)
                .OrderByDescending(q => q.CreatedAt)
                .Select(q => new StudentQuizSummaryDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    DurationInMinutes = q.DurationInMinutes,
                    QuestionCount = q.QuizQuestions.Count(qq => !qq.Question.IsDeleted)
                })
                .ToListAsync();


        public async Task<StudentQuizDto> GetQuizAsync(int quizId)
        {
            var quiz = await _context.Quizzes
                .AsNoTracking()
                .Include(q => q.QuizQuestions).ThenInclude(qq => qq.Question).ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync(q => q.Id == quizId && q.IsPublished)
                ?? throw new KeyNotFoundException($"Quiz with ID {quizId} not found.");

            return new StudentQuizDto
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Description = quiz.Description,
                DurationInMinutes = quiz.DurationInMinutes,
                Questions = quiz.QuizQuestions.OrderBy(qq => qq.Order).Select(qq => new StudentQuestionDto
                {
                    Id = qq.Question.Id,
                    Text = qq.Question.Text,
                    ImageUrl = qq.Question.ImageUrl,
                    Choices = qq.Question.Choices.OrderBy(c => c.Id)
                        .Select(c => new StudentChoiceDto { Id = c.Id, Text = c.Text })
                        .ToList()
                }).ToList()
            };
        }


        public async Task<QuizResultDto> SubmitQuizAsync(int quizId, int studentId, SubmitQuizDto dto)
        {

            var quiz = await _context.Quizzes
                .Include(q => q.QuizQuestions).ThenInclude(qq => qq.Question).ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync(q => q.Id == quizId && q.IsPublished)
                ?? throw new KeyNotFoundException($"Quiz with ID {quizId} not found.");

            var questions = quiz.QuizQuestions.OrderBy(qq => qq.Order).Select(qq => qq.Question).ToList();
            if (questions.Count == 0)
                throw new ArgumentException("This quiz has no questions.");


            if (dto.Answers.GroupBy(a => a.QuestionId).Any(g => g.Count() > 1))
                throw new ArgumentException("Each question can only be answered once.");

            var unknown = dto.Answers.Select(a => a.QuestionId).Except(questions.Select(q => q.Id)).ToList();
            if (unknown.Count > 0)
                throw new ArgumentException($"Questions [{string.Join(", ", unknown)}] do not belong to this quiz.");


            var answers = new List<StudentAnswer>();
            foreach (var question in questions)
            {
                var given = dto.Answers.FirstOrDefault(a => a.QuestionId == question.Id);

                Choice? selected = null;
                if (given?.SelectedChoiceId != null)
                {
                    selected = question.Choices.FirstOrDefault(c => c.Id == given.SelectedChoiceId)
                        ?? throw new ArgumentException(
                            $"Choice {given.SelectedChoiceId} does not belong to question {question.Id}.");
                }

                var correct = question.Choices.First(c => c.IsCorrect);

                answers.Add(new StudentAnswer
                {
                    QuestionId = question.Id,
                    SelectedChoiceId = selected?.Id,
                    IsCorrect = selected?.Id == correct.Id,
                    QuestionTextSnapshot = question.Text,
                    SelectedChoiceTextSnapshot = selected?.Text,
                    CorrectChoiceId = correct.Id,
                    CorrectChoiceTextSnapshot = correct.Text
                });
            }


            var attemptNumber = await _context.QuizSubmissions
                .CountAsync(s => s.QuizId == quizId && s.StudentId == studentId) + 1;

            var correctCount = answers.Count(a => a.IsCorrect);
            var now = DateTime.UtcNow;

            var submission = new QuizSubmission
            {
                Quiz = quiz,
                StudentId = studentId,
                AttemptNumber = attemptNumber,
                Status = SubmissionStatus.Submitted,
                StartedAt = now,
                SubmittedAt = now,
                TotalQuestions = questions.Count,
                CorrectAnswers = correctCount,
                IncorrectAnswers = questions.Count - correctCount,
                Score = correctCount,
                StudentAnswers = answers
            };


            _context.QuizSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Student {StudentId} submitted quiz {QuizId} (attempt {Attempt}): {Correct}/{Total}",
                studentId, quizId, attemptNumber, correctCount, questions.Count);

            return ResultMapper.ToResult(submission);
        }


        public async Task<List<AttemptSummaryDto>> GetMyResultsAsync(int studentId) =>
            await _context.QuizSubmissions
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(s => s.StudentId == studentId && s.Status == SubmissionStatus.Submitted)
                .OrderByDescending(s => s.SubmittedAt)
                .Select(s => new AttemptSummaryDto
                {
                    SubmissionId = s.Id,
                    QuizId = s.QuizId,
                    QuizTitle = s.Quiz.Title,
                    AttemptNumber = s.AttemptNumber,
                    Score = s.Score,
                    CorrectAnswers = s.CorrectAnswers,
                    IncorrectAnswers = s.IncorrectAnswers,
                    TotalQuestions = s.TotalQuestions,
                    SubmittedAt = s.SubmittedAt
                })
                .ToListAsync();


        public async Task<QuizResultDto> GetMyResultAsync(int submissionId, int studentId)
        {

            var submission = await _context.QuizSubmissions
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Include(s => s.Quiz)
                .Include(s => s.StudentAnswers)
                .FirstOrDefaultAsync(s => s.Id == submissionId
                                       && s.StudentId == studentId
                                       && s.Status == SubmissionStatus.Submitted)
                ?? throw new KeyNotFoundException($"Result with ID {submissionId} not found.");

            return ResultMapper.ToResult(submission);
        }
    }
}

