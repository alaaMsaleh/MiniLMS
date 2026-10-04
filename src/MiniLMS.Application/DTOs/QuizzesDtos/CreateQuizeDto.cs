using System.ComponentModel.DataAnnotations;

namespace MiniLMS.Application.DTOs.QuizzesDtos
{
    public class CreateQuizeDto
    {

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string? Description { get; set; }

        [Range(1, 600, ErrorMessage = "Duration must be between 1 and 600 minutes.")]
        public int DurationInMinutes { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "A quiz must contain at least one question.")]
        public List<int> QuestionIds { get; set; } = new();
    }
}
