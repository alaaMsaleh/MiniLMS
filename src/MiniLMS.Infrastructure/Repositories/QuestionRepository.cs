using Microsoft.EntityFrameworkCore;
using MiniLMS.Application.Interfaces;
using MiniLMS.Domain.Entities;
using MiniLMS.Infrastructure.DBContext;

namespace MiniLMS.Infrastructure.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicationDbContext _context;

        public QuestionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Question question)
        {
            await _context.Questions.AddAsync(question);
        }

        public async Task<Question?> GetByIdAsync(int id)
        {
            return await _context.Questions
             .Include(q => q.Choices)  //Eager Loading
             .FirstOrDefaultAsync(q => q.Id == id);
        }


        public async Task<List<Question>> GetAllAsync()
        {
            return await _context.Questions
             .Include(q => q.Choices)
             .AsNoTracking()  //readonly
             .ToListAsync();
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            await action();
            await transaction.CommitAsync();
        }


        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
