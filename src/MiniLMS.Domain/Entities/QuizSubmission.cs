namespace MiniLMS.Domain.Entities
{
    public class QuizSubmission
    {
        //Quiz From User
        public int Id { get; set; }
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;

        public int StudentId { get; set; }
        public User Student { get; set; } = null!;

        public int AttemptNumber { get; set; } = 1;

        public SubmissionStatus Status { get; set; } = SubmissionStatus.InProgress;

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SubmittedAt { get; set; }

        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int IncorrectAnswers { get; set; }

        // Optimistic concurrency (protects against double submit)
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();


    }
}
