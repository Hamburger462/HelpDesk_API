namespace HelpDesk.Data.Models
{
    public class Attachment
    {
        public Guid Id { get; set; }
        public required string ImageUrl { get; set; }
        
        public Guid TicketId { get; set; }
        public Ticket? Ticket { get; set; }
    }
}