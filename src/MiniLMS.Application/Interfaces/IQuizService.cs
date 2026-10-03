using MiniLMS.Application.DTOs.QuizzesDtos;

namespace MiniLMS.Application.Interfaces
{
    public interface IQuizService
    {
        Task<QuizResponseDto> CreateQuizAsync(CreateQuizeDto dto);
        Task<QuizResponseDto> GetQuizByIdAsync(int id);
        Task<IEnumerable<QuizResponseDto>> GetAllQuizzesAsync();

        Task<bool> DeleteQuizAsync(int id);

        Task<QuizResultDto> SubmitQuizAsync(int quizId, int studentId, SubmitQuizDto dto);
    }
}
