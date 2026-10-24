using FluentValidation;
using HelpDesk.Data.Dto;

namespace HelpDesk.Validation
{
    public class AttachmentFilterValidator : AbstractValidator<AttachmentFilter>
    {
        public AttachmentFilterValidator()
        {
            this.AddPagingRules();

            RuleFor(x => x.Search).MaximumLength(100);
            RuleFor(x => x.ContentType).MaximumLength(127);

            RuleFor(x => x)
                .Must(x => x.CreatedFrom is null || x.CreatedTo is null || x.CreatedFrom <= x.CreatedTo)
                .WithMessage("CreatedFrom must not be later than CreatedTo.");
        }
    }
}
