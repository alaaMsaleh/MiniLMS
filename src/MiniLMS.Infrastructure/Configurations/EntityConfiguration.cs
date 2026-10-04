using Microsoft.EntityFrameworkCore;
using MiniLMS.Domain.Entities;

namespace MiniLMS.Infrastructure.Configurations
{

    public static class EntityConfiguration
    {
        public static void ConfigureMiniLms(this ModelBuilder modelbuilder)
        {
            // ---------- QuizQuestion (join table) ----------
            modelbuilder.Entity<QuizQuestion>(e =>
            {
                e.HasKey(x => new { x.QuizId, x.QuestionId });

                e.HasOne(x => x.Quiz).WithMany(q => q.QuizQuestions)
                    .HasForeignKey(x => x.QuizId).OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Question).WithMany(q => q.QuizQuestions)
                    .HasForeignKey(x => x.QuestionId).OnDelete(DeleteBehavior.Restrict);
            });

            // ---------- Question / Choice ----------
            modelbuilder.Entity<Question>(e =>
            {
                e.Property(x => x.Text).IsRequired().HasMaxLength(1000);
                e.Property(x => x.ImageUrl).HasMaxLength(2048);
                e.HasQueryFilter(x => !x.IsDeleted);
            });

            modelbuilder.Entity<Choice>(e =>
            {
                e.Property(x => x.Text).IsRequired().HasMaxLength(500);

                e.HasOne(x => x.Question).WithMany(q => q.Choices)
                    .HasForeignKey(x => x.QuestionId).OnDelete(DeleteBehavior.Cascade);

                // unique index => one correct choice
                e.HasIndex(x => x.QuestionId)
                    .IsUnique()
                    .HasFilter("[IsCorrect] = 1 AND [IsDeleted] = 0");

                e.HasQueryFilter(x => !x.IsDeleted && !x.Question.IsDeleted);
            });

            // ---------- Quiz ----------
            modelbuilder.Entity<Quiz>(e =>
            {
                e.Property(x => x.Title).IsRequired().HasMaxLength(200);
                e.HasQueryFilter(x => !x.IsDeleted);
            });

            // ---------- QuizSubmission ----------
            modelbuilder.Entity<QuizSubmission>(e =>
            {

                e.Property(x => x.RowVersion).IsRowVersion();

                e.HasOne(x => x.Quiz).WithMany(q => q.Submissions)
                    .HasForeignKey(x => x.QuizId).OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Student).WithMany()
                    .HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.QuizId, x.StudentId, x.AttemptNumber }).IsUnique();
            });

            // ---------- StudentAnswers ----------
            modelbuilder.Entity<StudentAnswer>(e =>
            {
                e.HasOne(x => x.QuizSubmission).WithMany(s => s.StudentAnswers)
                    .HasForeignKey(x => x.QuizSubmissionId).OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Question).WithMany()
                    .HasForeignKey(x => x.QuestionId).OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.SelectedChoice).WithMany()
                    .HasForeignKey(x => x.SelectedChoiceId).OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.QuizSubmissionId, x.QuestionId }).IsUnique();

                e.Property(x => x.QuestionTextSnapshot).HasMaxLength(1000);
                e.Property(x => x.SelectedChoiceTextSnapshot).HasMaxLength(500);
                e.Property(x => x.CorrectChoiceTextSnapshot).HasMaxLength(500);
            });
        }
    }
}


