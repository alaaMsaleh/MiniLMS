namespace MiniLMS.Application.DTOs.StudentDto
{
    public class StudentOverviewDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int AttemptsCount { get; set; }
        public int QuizzesTaken { get; set; }
        public int TotalCorrect { get; set; }
        public int TotalIncorrect { get; set; }
        public double AveragePercentage { get; set; }
        public double BestPercentage { get; set; }
        public DateTime? LastSubmittedAt { get; set; }
    }
}
