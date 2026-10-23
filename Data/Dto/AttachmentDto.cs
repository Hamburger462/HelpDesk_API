namespace HelpDesk.Data.Dto
{
    public record AttachmentDto
    {
        public Guid Id  { get; set; }
        public required string ImageUrl  { get; set; }
    }
}