using HelpDesk.Data;
using HelpDesk.Services.Storage;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Features.Ticket.DeleteTicket
{
    public record DeleteTicketCommand(Guid Id) : IRequest<bool>;

    public class DeleteTicketHandler(AppDbContext context, IFileStorage storage) : IRequestHandler<DeleteTicketCommand, bool>
    {
        public async Task<bool> Handle(DeleteTicketCommand command, CancellationToken cancellationToken)
        {
            // The DB cascade removes messages and attachment rows but not the files, so collect the paths first.
            var paths = await context.Attachments
                .Where(a => a.TicketId == command.Id)
                .Select(a => a.StoragePath)
                .ToListAsync(cancellationToken);

            var deleted = await context.Tickets
                .Where(t => t.Id == command.Id)
                .ExecuteDeleteAsync(cancellationToken);
            if (deleted == 0) return false;

            await storage.DeleteAsync(paths, cancellationToken);
            return true;
        }
    }
}
