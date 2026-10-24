using FluentValidation;
using HelpDesk.Data.Dto;

namespace HelpDesk.Validation
{
    // TicketDto is used as input for create and update.
    // Create reads Title, Urgency and Messages; update reads Title and Urgency.
    public class TicketDtoValidator : AbstractValidator<TicketDto>
    {
        public TicketDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);   // matches the Tickets.Title column
            RuleFor(x => x.Urgency).IsInEnum();
            RuleForEach(x => x.Messages).SetValidator(new MessageDtoValidator());

            // Status, timestamps, CreatedById, Assignees and Attachments are ignored on input,
            // so they are not validated.
        }
    }
}
