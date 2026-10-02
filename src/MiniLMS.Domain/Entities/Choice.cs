namespace MiniLMS.Domain.Entities
{
    public class Choice
    {
        public int Id { get; set; }

        public int QuestionId { get; set; } //FK

        public string Text { get; set; }

        public bool IsCorrect { get; set; }
        public bool IsDeleted { get; set; }

        public Question Question { get; set; } = null!; //ONE to Many



    }
}