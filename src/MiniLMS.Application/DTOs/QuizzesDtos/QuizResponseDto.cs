namespace MiniLMS.Application.DTOs.QuizzesDtos
{
    public class QuizResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationInMinutes { get; set; }
        public List<QuestionResponseDto> Questions { get; set; } = new();

    }
}
