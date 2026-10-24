using FluentValidation;
using HelpDesk.Data.Dto;

namespace HelpDesk.Validation
{
    public class MessageDtoValidator : AbstractValidator<MessageDto>
    {
        public MessageDtoValidator()
        {
            RuleFor(x => x.Text).NotEmpty().MaximumLength(10_000);

            // Id, CreatedAt, AuthorId and AuthorFullName are output fields; the server sets them.
        }
    }
}
