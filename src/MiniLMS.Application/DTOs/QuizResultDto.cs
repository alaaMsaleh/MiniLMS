namespace MiniLMS.Application.DTOs
{
    public class QuizResultDto
    {
        public int SubmissionId { get; set; }
        public int QuizId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public int AttemptNumber { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int IncorrectAnswers { get; set; }
        public double Percentage =>
            TotalQuestions > 0 ? Math.Round((double)CorrectAnswers / TotalQuestions * 100, 2) : 0;
        public DateTime? SubmittedAt { get; set; }
        public List<AnswerResultDto> Answers { get; set; } = new();
    }
}
