using MiniLMS.Application.DTOs.ChoicesDtos;

namespace MiniLMS.Application.DTOs
{
    public class QuestionResponseDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } //option
        public List<ChoiceResponseDto> Choices { get; set; }
    }
}
