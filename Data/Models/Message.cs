namespace HelpDesk.Data.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public required string Text { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public Guid TicketId { get; set; }
        public Ticket? Ticket { get; set; }
    }
}