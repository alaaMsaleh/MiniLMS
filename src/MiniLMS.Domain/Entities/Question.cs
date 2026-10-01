namespace MiniLMS.Domain.Entities
{
    public class Questions
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public string ImageUrl { get; set; }

        public bool IsDeleted { get; set; }
    }
}
