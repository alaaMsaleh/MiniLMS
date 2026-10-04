using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniLMS.Application.DTOs;
using MiniLMS.Application.DTOs.ChoicesDtos;
using MiniLMS.Application.DTOs.QuizzesDtos;
using MiniLMS.Application.Interfaces;
using MiniLMS.Domain.Entities;
using MiniLMS.Infrastructure.DBContext;

namespace MiniLMS.Infrastructure.Services
{
    public class QuizService : IQuizService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<QuizService> _logger;

        public QuizService(ApplicationDbContext context, ILogger<QuizService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<QuizResponseDto> CreateQuizAsync(CreateQuizeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title is required.");

            var ids = dto.QuestionIds.Distinct().ToList();
            if (ids.Count == 0)
                throw new ArgumentException("A quiz must contain at least one question.");

            //  query filter
            var existing = await _context.Questions
                .Where(q => ids.Contains(q.Id))
                .Select(q => q.Id)
                .ToListAsync();

            var missing = ids.Except(existing).ToList();
            if (missing.Count > 0)
                throw new KeyNotFoundException($"Questions with IDs [{string.Join(", ", missing)}] do not exist.");

            var quiz = new Quiz
            {
                Title = dto.Title.Trim(),
                Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                DurationInMinutes = dto.DurationInMinutes,
                IsPublished = false,
                QuizQuestions = ids.Select((id, i) => new QuizQuestion { QuestionId = id, Order = i + 1 }).ToList()
            };

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Quiz {QuizId} created with {Count} questions", quiz.Id, ids.Count);

            return (await GetQuizByIdAsync(quiz.Id))!;
        }

        public async Task<QuizResponseDto?> GetQuizByIdAsync(int id)
        {
            var quiz = await _context.Quizzes
                .AsNoTracking()
                .Include(q => q.QuizQuestions).ThenInclude(qq => qq.Question).ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz is null) return null;

            return new QuizResponseDto
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Description = quiz.Description,
                DurationInMinutes = quiz.DurationInMinutes,
                IsPublished = quiz.IsPublished,
                Questions = quiz.QuizQuestions.OrderBy(qq => qq.Order).Select(qq => new QuestionResponseDto
                {
                    Id = qq.Question.Id,
                    Text = qq.Question.Text,
                    ImageUrl = qq.Question.ImageUrl,
                    Choices = qq.Question.Choices.OrderBy(c => c.Id).Select(c => new ChoiceResponseDto
                    {
                        Id = c.Id,
                        Text = c.Text,
                        IsCorrect = c.IsCorrect
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<List<QuizSummaryDto>> GetAllQuizzesAsync() =>
            await _context.Quizzes
                .AsNoTracking()
                .OrderByDescending(q => q.CreatedAt)
                .Select(q => new QuizSummaryDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    DurationInMinutes = q.DurationInMinutes,
                    IsPublished = q.IsPublished,
                    QuestionCount = q.QuizQuestions.Count(qq => !qq.Question.IsDeleted),
                    CreatedAt = q.CreatedAt
                })
                .ToListAsync();

        public async Task SetPublishedAsync(int id, bool isPublished)
        {
            var quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == id)
                ?? throw new KeyNotFoundException($"Quiz with ID {id} not found.");

            if (isPublished)
            {
                var activeQuestions = await _context.QuizQuestions
                    .CountAsync(qq => qq.QuizId == id && !qq.Question.IsDeleted);

                if (activeQuestions == 0)
                    throw new ArgumentException("Cannot publish a quiz that has no active questions.");
            }

            quiz.IsPublished = isPublished;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Quiz {QuizId} {Action}", id, isPublished ? "published" : "unpublished");
        }

        public async Task DeleteQuizAsync(int id)
        {
            var quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == id)
                ?? throw new KeyNotFoundException($"Quiz with ID {id} not found.");

            quiz.IsDeleted = true; // Soft Delete
            quiz.IsPublished = false;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Quiz {QuizId} soft-deleted", id);
        }
    }
}