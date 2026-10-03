namespace MiniLMS.Application.DTOs.QuizzesDtos
{
    public class SubmitQuizDto
    {
        public List<QuestionAnswerDto> Answers { get; set; } = new();
    }
}
