namespace HelpDesk.Data.Models
{
    public class Ticket
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public required Message Message { get; set; }

        public Attachment? Attachment { get; set; }
    }
}