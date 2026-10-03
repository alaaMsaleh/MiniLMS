namespace MiniLMS.Domain.Entities
{
    public class StudentAnswer
    {
        public int Id { get; set; }
        public int QuizSubmissionId { get; set; }
        public QuizSubmission QuizSubmission { get; set; } = null!;

        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
        // student left the question unanswered
        public int? SelectedChoiceId { get; set; }
        public Choice? SelectedChoice { get; set; } = null!;

        public bool IsCorrect { get; set; }

        //Snapshot 
        public string QuestionTextSnapshot { get; set; } = string.Empty;
        public string? SelectedChoiceTextSnapshot { get; set; }
        public int CorrectChoiceId { get; set; }
        public string CorrectChoiceTextSnapshot { get; set; } = string.Empty;
    }
}
