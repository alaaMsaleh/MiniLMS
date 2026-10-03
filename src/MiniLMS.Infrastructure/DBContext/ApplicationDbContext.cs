
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniLMS.Domain.Entities;

namespace MiniLMS.Infrastructure.DBContext
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }



        public DbSet<Question> Questions { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizSubmission> QuizSubmissions { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<StudentAnswer> StudentAnswer { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<QuizQuestion>()
        .HasKey(qq => new { qq.QuizId, qq.QuestionId });

            modelBuilder.Entity<QuizQuestion>()
                .HasOne(qq => qq.Quiz)
                .WithMany(q => q.QuizQuestions)
                .HasForeignKey(qq => qq.QuizId);

            modelBuilder.Entity<QuizQuestion>()
                .HasOne(qq => qq.Question)
                .WithMany()

                .HasForeignKey(qq => qq.QuestionId);



        }

    }
}