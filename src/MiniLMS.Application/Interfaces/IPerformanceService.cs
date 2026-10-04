using MiniLMS.Application.DTOs;
using MiniLMS.Application.DTOs.StudentDto;

namespace MiniLMS.Application.Interfaces
{
    public interface IPerformanceService
    {

        Task<List<StudentPerformanceDto>> GetAttemptsAsync(int? quizId, int? studentId);
        Task<List<StudentOverviewDto>> GetStudentsOverviewAsync();
        Task<QuizResultDto> GetSubmissionAsync(int submissionId);
    }

}
