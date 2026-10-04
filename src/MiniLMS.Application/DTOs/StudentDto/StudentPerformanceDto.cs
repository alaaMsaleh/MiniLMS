namespace MiniLMS.Application.DTOs.StudentDto
{
    public class StudentPerformanceDto : AttemptSummaryDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
    }
}
