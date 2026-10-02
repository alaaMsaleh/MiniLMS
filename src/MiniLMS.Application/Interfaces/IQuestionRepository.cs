using MiniLMS.Domain.Entities;

namespace MiniLMS.Application.Interfaces
{
    public interface IQuestionRepository
    {
        Task AddAsync(Question question);
        Task<List<Question>> GetAllAsync();
        Task<Question?> GetByIdAsync(int id);
        void Update(Question question);
        void Delete(Question question);
        Task<bool> SaveChangesAsync();
    }
}
