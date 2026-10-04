

using MiniLMS.Application.DTOs;
using MiniLMS.Domain.Entities;

namespace MiniLMS.Infrastructure.Services
{
    internal static class ResultMapper
    {
        public static QuizResultDto ToResult(QuizSubmission s) => new()
        {
            SubmissionId = s.Id,
            QuizId = s.QuizId,
            QuizTitle = s.Quiz.Title,
            AttemptNumber = s.AttemptNumber,
            Score = s.Score,
            TotalQuestions = s.TotalQuestions,
            CorrectAnswers = s.CorrectAnswers,
            IncorrectAnswers = s.IncorrectAnswers,
            SubmittedAt = s.SubmittedAt,
            Answers = s.StudentAnswers.OrderBy(a => a.Id).Select(a => new AnswerResultDto
            {
                QuestionId = a.QuestionId,
                QuestionText = a.QuestionTextSnapshot,
                SelectedChoiceText = a.SelectedChoiceTextSnapshot,
                CorrectChoiceText = a.CorrectChoiceTextSnapshot,
                IsCorrect = a.IsCorrect
            }).ToList()
        };
    }
}
