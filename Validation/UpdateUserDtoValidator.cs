using FluentValidation;
using HelpDesk.Data.Dto;

namespace HelpDesk.Validation
{
    // Every field is optional (partial update), but any field that is sent must be valid.
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(x => x)
                .Must(x => x.FullName is not null || x.Email is not null || x.Password is not null)
                .WithMessage("Provide at least one field to update.");

            RuleFor(x => x.FullName!)
                .NotEmpty().MaximumLength(200)
                .When(x => x.FullName is not null);

            RuleFor(x => x.Email!)
                .Cascade(CascadeMode.Stop).NotEmpty().ValidEmail()
                .When(x => x.Email is not null);

            RuleFor(x => x.Password!)
                .Cascade(CascadeMode.Stop).NotEmpty().ValidPassword()
                .When(x => x.Password is not null);
        }
    }
}
