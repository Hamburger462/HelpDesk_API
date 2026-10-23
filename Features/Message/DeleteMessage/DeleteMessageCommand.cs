using HelpDesk.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Features.Message.DeleteMessage
{
    // Returns false when the message does not exist on that ticket.
    public record DeleteMessageCommand(Guid TicketId, Guid Id) : IRequest<bool>;

    public class DeleteMessageHandler(AppDbContext context) : IRequestHandler<DeleteMessageCommand, bool>
    {
        public async Task<bool> Handle(DeleteMessageCommand command, CancellationToken cancellationToken)
        {
            var deleted = await context.Messages
                .Where(m => m.Id == command.Id && m.TicketId == command.TicketId)
                .ExecuteDeleteAsync(cancellationToken);

            return deleted > 0;
        }
    }
}
