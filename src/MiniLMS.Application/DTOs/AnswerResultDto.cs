namespace MiniLMS.Application.DTOs
{
    public class AnswerResultDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string? SelectedChoiceText { get; set; }
        public string CorrectChoiceText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
