using Microsoft.EntityFrameworkCore;
using MiniLMS.Application.DTOs;
using MiniLMS.Application.DTOs.StudentDto;
using MiniLMS.Application.Interfaces;
using MiniLMS.Domain.Entities;
using MiniLMS.Infrastructure.DBContext;

namespace MiniLMS.Infrastructure.Services
{
    public class PerformanceService : IPerformanceService
    {
        private readonly ApplicationDbContext _context;

        public PerformanceService(ApplicationDbContext context) => _context = context;


        public async Task<List<StudentPerformanceDto>> GetAttemptsAsync(int? quizId, int? studentId) =>
            await _context.QuizSubmissions
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(s => s.Status == SubmissionStatus.Submitted
                         && (quizId == null || s.QuizId == quizId)
                         && (studentId == null || s.StudentId == studentId))
                .OrderByDescending(s => s.SubmittedAt)
                .Select(s => new StudentPerformanceDto
                {
                    SubmissionId = s.Id,
                    StudentId = s.StudentId,
                    StudentName = s.Student.FullName,
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


        public async Task<List<StudentOverviewDto>> GetStudentsOverviewAsync()
        {

            var rows = await _context.QuizSubmissions
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(s => s.Status == SubmissionStatus.Submitted)
                .Select(s => new
                {
                    s.StudentId,
                    StudentName = s.Student.FullName,
                    s.QuizId,
                    s.CorrectAnswers,
                    s.IncorrectAnswers,
                    s.TotalQuestions,
                    s.SubmittedAt
                })
                .ToListAsync();

            return rows
                .GroupBy(r => r.StudentId)
                .Select(g => new StudentOverviewDto
                {
                    StudentId = g.Key,
                    StudentName = g.First().StudentName,
                    AttemptsCount = g.Count(),
                    QuizzesTaken = g.Select(r => r.QuizId).Distinct().Count(),
                    TotalCorrect = g.Sum(r => r.CorrectAnswers),
                    TotalIncorrect = g.Sum(r => r.IncorrectAnswers),
                    AveragePercentage = Math.Round(g.Average(r => Percent(r.CorrectAnswers, r.TotalQuestions)), 2),
                    BestPercentage = Math.Round(g.Max(r => Percent(r.CorrectAnswers, r.TotalQuestions)), 2),
                    LastSubmittedAt = g.Max(r => r.SubmittedAt)
                })
                .OrderByDescending(o => o.AveragePercentage)
                .ToList();
        }


        public async Task<QuizResultDto> GetSubmissionAsync(int submissionId)
        {
            var submission = await _context.QuizSubmissions
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Include(s => s.Quiz)
                .Include(s => s.StudentAnswers)
                .FirstOrDefaultAsync(s => s.Id == submissionId && s.Status == SubmissionStatus.Submitted)
                ?? throw new KeyNotFoundException($"Submission with ID {submissionId} not found.");

            return ResultMapper.ToResult(submission);
        }

        private static double Percent(int correct, int total) =>
            total > 0 ? (double)correct / total * 100 : 0;
    }
}
