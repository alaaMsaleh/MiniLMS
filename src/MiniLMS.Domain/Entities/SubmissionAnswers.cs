namespace MiniLMS.Domain.Entities
{
    public class SubmissionAnswers
    {
        public int Id { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }
        public int QuizSubmissionId { get; set; }
        public int SelectedChiceId { get; set; }



    }
}
