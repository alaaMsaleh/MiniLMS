using MiniLMS.Application.DTOs.QuizzesDtos;

namespace MiniLMS.Application.Interfaces
{
    public interface IQuizService
    {
        Task<QuizResponseDto> CreateQuizAsync(CreateQuizeDto dto);
        Task<QuizResponseDto?> GetQuizByIdAsync(int id);
        Task<List<QuizSummaryDto>> GetAllQuizzesAsync();
        Task SetPublishedAsync(int id, bool isPublished);
        Task DeleteQuizAsync(int id);
    }
}
