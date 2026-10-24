namespace HelpDesk.Data.Dto
{
    // Slim user shape for embedding in tickets (e.g. the list of assignees).
    // Deliberately has no ticket list, so tickets and users never nest into each other.
    public record UserSummaryDto
    {
        public Guid Id { get; set; }
        public required string FullName { get; set; }
        public required string Role { get; set; }
    }
}
