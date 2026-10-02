using Microsoft.EntityFrameworkCore;
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

        public QuizService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<QuizResponseDto> CreateQuizAsync(CreateQuizeDto dto)
        {
            if (dto.QuestionIds == null || !dto.QuestionIds.Any())
            {
                throw new ArgumentException("Quiz must contain at least one question.");
            }

            //check question at Db

            var exisitngQuestionIds = await _context.Questions
                .Where(q => dto.QuestionIds.Contains(q.Id))
                .Select(q => q.Id)
                .ToListAsync();

            var missingIds = dto.QuestionIds
                .Except(exisitngQuestionIds).ToList();

            if (missingIds.Any())
            {

                throw new KeyNotFoundException($"Questions with IDs [{string.Join(", ", missingIds)}] do not exist.");

            }
            var quiz = new Quiz
            {
                Title = dto.Title,
                Description = dto.Description,
                DurationInMinutes = dto.DurationInMinutes,
                QuizQuestions = dto.QuestionIds.Select(qId => new QuizQuestion
                {
                    QuestionId = qId
                }).ToList()
            };

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            return (await GetQuizByIdAsync(quiz.Id))!;

        }

        public async Task<bool> DeleteQuizAsync(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null) return false;

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<QuizResponseDto>> GetAllQuizzesAsync()
        {
            return await _context.Quizzes
                .Include(q => q.QuizQuestions)
                    .ThenInclude(qq => qq.Question)
                .AsNoTracking()
                .Select(q => new QuizResponseDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    DurationInMinutes = q.DurationInMinutes,
                    Questions = q.QuizQuestions.Select(qq => new QuestionResponseDto
                    {
                        Id = qq.Question.Id,
                        Text = qq.Question.Text,
                        ImageUrl = qq.Question.ImageUrl
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<QuizResponseDto?> GetQuizByIdAsync(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.QuizQuestions)
                 .ThenInclude(q => q.Question)
                .ThenInclude(q => q.Choices)
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null) return null;

            return new QuizResponseDto
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Description = quiz.Description,
                DurationInMinutes = quiz.DurationInMinutes,
                Questions = quiz.QuizQuestions?.Select(qq => new QuestionResponseDto
                {
                    Id = qq.Question.Id,
                    Text = qq.Question.Text,
                    ImageUrl = qq.Question.ImageUrl,
                    Choices = qq.Question.Choices?.Select(c => new ChoiceResponseDto
                    {
                        Id = c.Id,
                        Text = c.Text,
                        IsCorrect = c.IsCorrect
                    }).ToList() ?? new List<ChoiceResponseDto>()
                }).ToList() ?? new List<QuestionResponseDto>()
            };

        }
    }
}
