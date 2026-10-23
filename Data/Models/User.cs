namespace HelpDesk.Data.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PassHash { get; set; }
        public string Role { get; set; } = Roles.User;

        public List<Ticket> CreatedTickets { get; set; } = new();    // tickets this user sent
        public List<Ticket> AssignedTickets { get; set; } = new();   // tickets this user responds to
    }
}
