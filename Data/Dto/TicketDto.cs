using HelpDesk.Data.Models;

namespace HelpDesk.Data.Dto
{
    public record TicketDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required MessageDto Message { get; set; }
        public AttachmentDto? Attachment { get; set; }
    }
}