using MiniLMS.Domain.Entities;

namespace MiniLMS.Application.Interfaces
{
    public interface IQuestionRepository
    {
        Task AddAsync(Question question);
        Task<List<Question>> GetAllAsync();
        Task<Question?> GetByIdAsync(int id);
        Task ExecuteInTransactionAsync(Func<Task> action);
        Task<bool> SaveChangesAsync();
    }
}
