namespace MiniLMS.Domain.Entities
{
    public class Quiz
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
        //only student see published quiz
        public bool IsPublished { get; set; }
        public int DurationInMinutes { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();

        public ICollection<QuizSubmission> Submissions { get; set; } = new List<QuizSubmission>();
    }
}
