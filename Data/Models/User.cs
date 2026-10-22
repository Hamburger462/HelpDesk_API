namespace HelpDesk.Data.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PassHash { get; set; }
        public string Role { get; set; } = "User";
        public List<Ticket> Tickets { get; set; } = new();
    }
}