namespace Gem.DAL.Domain
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int TokenUsed { get; set; }
        public string Model { get; set; }
        public string Role  { get; set; }
        public DateTime CreatedAt = DateTime.UtcNow;
    }
}
