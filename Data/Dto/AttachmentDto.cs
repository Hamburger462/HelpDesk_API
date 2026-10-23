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
}