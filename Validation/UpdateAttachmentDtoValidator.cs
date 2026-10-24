using FluentValidation;
using HelpDesk.Data.Dto;

namespace HelpDesk.Validation
{
    public class UpdateAttachmentDtoValidator : AbstractValidator<UpdateAttachmentDto>
    {
        public UpdateAttachmentDtoValidator()
        {
            // The handler appends the stored extension, so the length limit is re-checked there too.
            RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
        }
    }
}
