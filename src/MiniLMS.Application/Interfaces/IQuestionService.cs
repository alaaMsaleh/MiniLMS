using MiniLMS.Application.DTOs;

namespace MiniLMS.Application.Interfaces
{
    public interface IQuestionService
    {
        Task<QuestionResponseDto> CreateQuestionAsync(QuestionResponceDto dto);
        Task<List<QuestionResponseDto>> GetAllQuestionsAsync();
        Task<QuestionResponseDto?> GetQuestionByIdAsync(int id);
        Task UpdateQuestionAsync(int id, QuestionResponceDto dto);
        Task DeleteQuestionAsync(int id);
    }
}
