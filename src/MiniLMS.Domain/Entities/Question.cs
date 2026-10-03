namespace MiniLMS.Domain.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }
        public ICollection<Choice> Choices { get; set; } = new HashSet<Choice>();

        public ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();
    }
}
