using System.ComponentModel.DataAnnotations;

namespace MiniLMS.Application.DTOs
{
    public class QuestionAnswerDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "QuestionId is invalid.")]
        public int QuestionId { get; set; }


        public int? SelectedChoiceId { get; set; }
    }
}
