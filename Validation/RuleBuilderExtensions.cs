using FluentValidation;

namespace HelpDesk.Validation
{
    // Rules shared by more than one validator.
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilderOptions<T, string> ValidEmail<T>(this IRuleBuilder<T, string> rule) =>
            rule.EmailAddress()
                .MaximumLength(256);   // matches the Users.Email column

        public static IRuleBuilderOptions<T, string> ValidPassword<T>(this IRuleBuilder<T, string> rule) =>
            rule.MinimumLength(8)
                .MaximumLength(128)    // upper bound so huge strings never reach the hasher
                .Matches("[A-Za-z]").WithMessage("Password must contain at least one letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.");
    }
}
