namespace MiniLMS.Application.DTOs.QuizzesDtos
{
    public class QuizSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationInMinutes { get; set; }
        public bool IsPublished { get; set; }
        public int QuestionCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
