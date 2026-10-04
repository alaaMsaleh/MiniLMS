using MiniLMS.Application.DTOs.ChoicesDtos;
using System.ComponentModel.DataAnnotations;

namespace MiniLMS.Application.DTOs.QuestionDtos
{
    public class SaveQuestionDto
    {
        [Required(ErrorMessage = "Question text is required.")]
        [StringLength(1000, ErrorMessage = "Question text cannot exceed 1000 characters.")]
        public string Text { get; set; } = string.Empty;

        [StringLength(2048, ErrorMessage = "Image URL cannot exceed 2048 characters.")]
        public string? ImageUrl { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "A question must have at least 2 choices.")]
        [MaxLength(6, ErrorMessage = "A question cannot have more than 6 choices.")]
        public List<CreateChoiceDto> Choices { get; set; } = new();
    }

}
