namespace HelpDesk.Data.Dto
{
    public record MessageDto
    {
        public Guid Id { get; set; }
        public required string Text { get; set; }
    }
}