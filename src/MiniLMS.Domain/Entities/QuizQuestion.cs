namespace MiniLMS.Domain.Entities
{

    //Join_Table
    public class QuizQuestion
    {
        public int QuizId { get; set; } //FK1
        public Quiz Quiz { get; set; } = null!;

        public int QuestionId { get; set; } //FK2
        public Question Question { get; set; } = null!;

        // Display order of the question inside the quiz
        public int Order { get; set; }
    }
}
