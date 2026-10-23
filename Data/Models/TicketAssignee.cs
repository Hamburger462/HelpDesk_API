namespace HelpDesk.Data.Models
{
    // Join entity: an operator responding to a ticket.
    // The database cannot require that UserId is an operator, so check Role when assigning.
    public class TicketAssignee
    {
        public Guid TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public DateTimeOffset AssignedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
