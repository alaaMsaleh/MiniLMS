namespace MiniLMS.Application.DTOs.QuizzesDtos
{
    public class CreateQuizeDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationInMinutes { get; set; }

        //Just_Number 
        public List<int> QuestionIds { get; set; } = new List<int>();
    }
}
