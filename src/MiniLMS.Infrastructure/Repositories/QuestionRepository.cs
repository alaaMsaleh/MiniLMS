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

        public void Delete(Question question)
        {
            _context.Questions.Remove(question); //change status
        }

        public void Update(Question question)
        {
            _context.Questions.Update(question);
        }


        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
