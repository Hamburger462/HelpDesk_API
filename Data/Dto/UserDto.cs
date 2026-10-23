namespace HelpDesk.Data.Dto
{
    public record UserDto
    {
        public Guid Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public string Role { get; set; } = "User";
        public List<TicketDto> Tickets { get; set; } = new();
    }

    public record CreateUserDto: UserDto
    {
        public new Guid? Id  { get; set; }
        public required string Password { get; set; }
    }

    public record UpdateUserDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}