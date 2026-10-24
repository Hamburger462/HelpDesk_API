using FluentValidation;
using HelpDesk.Data.Dto;

namespace HelpDesk.Validation
{
    public class TicketFilterValidator : AbstractValidator<TicketFilter>
    {
        public TicketFilterValidator()
        {
            this.AddPagingRules();

            RuleFor(x => x.Search).MaximumLength(100);
            RuleFor(x => x.Status).IsInEnum();
            RuleFor(x => x.Urgency).IsInEnum();

            RuleFor(x => x)
                .Must(x => !(x.Unassigned == true && x.AssigneeId is not null))
                .WithMessage("Unassigned cannot be combined with AssigneeId.");

            RuleFor(x => x)
                .Must(x => x.CreatedFrom is null || x.CreatedTo is null || x.CreatedFrom <= x.CreatedTo)
                .WithMessage("CreatedFrom must not be later than CreatedTo.");
        }
    }
}
