namespace HelpDesk.Data.Dto
{
    public record MessageDto
    {
        public Guid Id { get; set; }
        public required string Text { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public Guid AuthorId { get; set; }
        public string? AuthorFullName { get; set; }
    }
}
