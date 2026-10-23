using HelpDesk.Data;
using HelpDesk.Data.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Features.Message.UpdateMessage
{
    // Returns false when the message does not exist on that ticket.
    public record UpdateMessageCommand(Guid TicketId, Guid Id, MessageDto Dto) : IRequest<bool>;

    public class UpdateMessageHandler(AppDbContext context) : IRequestHandler<UpdateMessageCommand, bool>
    {
        public async Task<bool> Handle(UpdateMessageCommand command, CancellationToken cancellationToken)
        {
            var message = await context.Messages
                .FirstOrDefaultAsync(m => m.Id == command.Id && m.TicketId == command.TicketId, cancellationToken);
            if (message is null) return false;

            message.Text = command.Dto.Text;
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
