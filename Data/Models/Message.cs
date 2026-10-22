namespace HelpDesk.Data.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public required string Text { get; set; }
        
        public Guid TicketId { get; set; }
        public required Ticket Ticket { get; set; }
    }
}