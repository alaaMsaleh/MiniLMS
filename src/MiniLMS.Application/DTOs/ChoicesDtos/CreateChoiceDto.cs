namespace MiniLMS.Application.DTOs.ChoicesDtos
{
    public class CreateChoiceDto
    {
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
