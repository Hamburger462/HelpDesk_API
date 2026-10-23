namespace HelpDesk.Data.Models
{
    public class Attachment
    {
        public Guid Id { get; set; }
        public required string FileName { get; set; }      // original name, for display only
        public required string ContentType { get; set; }
        public long SizeBytes { get; set; }
        public required string StoragePath { get; set; }   // e.g. tickets/{ticketId}/{id}.pdf
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public Guid UploadedById { get; set; }
        public User? UploadedBy { get; set; }

        public Guid TicketId { get; set; }
        public Ticket? Ticket { get; set; }
    }
}
