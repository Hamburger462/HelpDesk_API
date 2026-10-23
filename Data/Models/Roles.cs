namespace HelpDesk.Data.Models
{
    public static class Roles
    {
        public const string User = "User";          // creates tickets
        public const string Operator = "Operator";  // can be assigned to and respond to tickets
        public const string Admin = "Admin";        // manages everything

        public static readonly string[] All = [User, Operator, Admin];
    }
}
