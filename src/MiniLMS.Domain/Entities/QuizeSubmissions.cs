namespace MiniLMS.Domain.Entities
{
    public class QuizeSubmissions
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public int StudentId { get; set; }
        public int SubmittedId { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswersCount { get; set; }
        public int InCorrectAnswersCount { get; set; }


    }
}
