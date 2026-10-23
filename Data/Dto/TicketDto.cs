namespace HelpDesk.Data.Dto
{
    public record TicketDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public List<MessageDto> Messages { get; set; } = new();
        public List<AttachmentDto> Attachments { get; set; } = new();
    }
}