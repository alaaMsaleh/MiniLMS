using MiniLMS.Application.DTOs;
using MiniLMS.Application.DTOs.QuizzesDtos;
using MiniLMS.Application.DTOs.StudentDto;

namespace MiniLMS.Application.Interfaces
{
    public interface IStudentQuizService
    {
        Task<List<StudentQuizSummaryDto>> GetAvailableQuizzesAsync();
        Task<StudentQuizDto> GetQuizAsync(int quizId);
        Task<QuizResultDto> SubmitQuizAsync(int quizId, int studentId, SubmitQuizDto dto);

        Task<List<AttemptSummaryDto>> GetMyResultsAsync(int studentId);
        Task<QuizResultDto> GetMyResultAsync(int submissionId, int studentId);
    }
}
