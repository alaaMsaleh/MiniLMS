using System.ComponentModel.DataAnnotations;

namespace MiniLMS.Application.DTOs.ChoicesDtos
{
    public class CreateChoiceDto
    {
        [Required(ErrorMessage = "Choice text is required.")]
        [StringLength(500, ErrorMessage = "Choice text cannot exceed 500 characters.")]
        public string Text { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }
    }
}
