namespace MiniLMS.Application.DTOs.QuizzesDtos
{
    public class StartQuizResponseDto
    {
        public int SubmissionId { get; set; }
        public int QuizId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int AttemptNumber { get; set; }
        public int DurationInMinutes { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public List<StudentQuestionDto> Questions { get; set; } = new();
    }
}
