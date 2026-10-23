using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Data.Dto
{
    public record AttachmentDto
    {
        public Guid Id { get; set; }
        public required string FileName { get; set; }
        public required string ContentType { get; set; }
        public long SizeBytes { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public record UpdateAttachmentDto
    {
        [Required, StringLength(255, MinimumLength = 1)]
        public required string FileName { get; set; }
    }
}