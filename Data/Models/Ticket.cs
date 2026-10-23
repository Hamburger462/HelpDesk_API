namespace HelpDesk.Data.Models
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public List<Message> Messages { get; set; } = new();
        public List<Attachment> Attachments { get; set; } = new();
    }
}