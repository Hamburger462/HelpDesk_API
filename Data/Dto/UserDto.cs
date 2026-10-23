namespace HelpDesk.Data.Dto
{
    public record UserDto
    {
        public Guid Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public List<TicketDto> Tickets { get; set; } = new();
    }

    public record CreateUserDto: UserDto
    {
        public new Guid? Id  { get; set; }
        public required string Password { get; set; }
    }
}