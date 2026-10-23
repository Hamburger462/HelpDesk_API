namespace HelpDesk.Data.Models
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public TicketStatus Status { get; set; } = TicketStatus.Open;
        public TicketUrgency Urgency { get; set; } = TicketUrgency.Normal;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? ResolvedAt { get; set; }

        // The sender
        public Guid CreatedById { get; set; }
        public User? CreatedBy { get; set; }

        // The responders (users with the Operator or Admin role)
        public List<User> Assignees { get; set; } = new();

        public List<Message> Messages { get; set; } = new();
        public List<Attachment> Attachments { get; set; } = new();
    }
}
