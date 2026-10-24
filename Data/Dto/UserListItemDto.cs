namespace HelpDesk.Data.Dto
{
    // List shape for users: no ticket list, so a page of users stays small.
    public record UserListItemDto
    {
        public Guid Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
    }
}
