using MiniLMS.Application.DTOs;
using MiniLMS.Application.DTOs.QuestionDtos;

namespace MiniLMS.Application.Interfaces
{
    public interface IQuestionService
    {
        Task<QuestionResponseDto> CreateQuestionAsync(SaveQuestionDto dto);
        Task<List<QuestionResponseDto>> GetAllQuestionsAsync();
        Task<QuestionResponseDto?> GetQuestionByIdAsync(int id);
        Task<QuestionResponseDto> UpdateQuestionAsync(int id, SaveQuestionDto dto);
        Task DeleteQuestionAsync(int id);
    }
}
