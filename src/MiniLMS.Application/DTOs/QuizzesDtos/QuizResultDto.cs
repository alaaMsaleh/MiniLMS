namespace MiniLMS.Application.DTOs.QuizzesDtos
{
    public class QuizResultDto
    {
        public int SubmissionId { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int IncorrectAnswers { get; set; }
        public double Percentage => TotalQuestions > 0 ? (double)Score / TotalQuestions * 100 : 0;
        public DateTime SubmittedAt { get; set; }
    }
}
