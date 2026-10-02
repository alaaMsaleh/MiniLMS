using MiniLMS.Application.DTOs.ChoicesDtos;

namespace MiniLMS.Application.DTOs
{
    public class QuestionResponceDto
    {
        public string Text { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } //option
        public List<CreateChoiceDto> Choices { get; set; } = new();
    }
}
