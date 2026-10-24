using FluentValidation;
using HelpDesk.Data.Dto;

namespace HelpDesk.Validation
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Email).Cascade(CascadeMode.Stop).NotEmpty().ValidEmail();
            RuleFor(x => x.Password).Cascade(CascadeMode.Stop).NotEmpty().ValidPassword();

            // Id, Role and Tickets are inherited from UserDto but ignored when creating a user.
        }
    }
}
