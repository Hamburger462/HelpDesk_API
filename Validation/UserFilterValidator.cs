using FluentValidation;
using HelpDesk.Data.Dto;
using HelpDesk.Data.Models;

namespace HelpDesk.Validation
{
    public class UserFilterValidator : AbstractValidator<UserFilter>
    {
        public UserFilterValidator()
        {
            this.AddPagingRules();

            RuleFor(x => x.Search).MaximumLength(100);

            RuleFor(x => x.Role!)
                .Must(role => Roles.All.Contains(role))
                .WithMessage($"Role must be one of: {string.Join(", ", Roles.All)}.")
                .When(x => !string.IsNullOrEmpty(x.Role));
        }
    }
}
