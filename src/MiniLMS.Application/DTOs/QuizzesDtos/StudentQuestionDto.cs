namespace MiniLMS.Application.DTOs.QuizzesDtos
{
    public class StudentQuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public List<StudentChoiceDto> Choices { get; set; } = new();
    }
}
