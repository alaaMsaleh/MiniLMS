using System.ComponentModel.DataAnnotations;

namespace MiniLMS.Application.DTOs.QuizzesDtos
{
    public class SubmitQuizDto
    {
        [Required]
        public List<QuestionAnswerDto> Answers { get; set; } = new();
    }
}
