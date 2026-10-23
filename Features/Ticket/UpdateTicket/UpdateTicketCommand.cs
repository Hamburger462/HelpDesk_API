using HelpDesk.Data;
using HelpDesk.Data.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Features.Ticket.UpdateTicket
{
    public enum UpdateTicketStatus { Updated, NotFound, UserNotFound }

    public record UpdateTicketCommand(Guid Id, TicketDto Dto) : IRequest<UpdateTicketStatus>;

    public class UpdateTicketHandler(AppDbContext context) : IRequestHandler<UpdateTicketCommand, UpdateTicketStatus>
    {
        public async Task<UpdateTicketStatus> Handle(UpdateTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = await context.Tickets.FirstOrDefaultAsync(t => t.Id == command.Id, cancellationToken);
            if (ticket is null) return UpdateTicketStatus.NotFound;

            if (!await context.Users.AnyAsync(u => u.Id == command.Dto.UserId, cancellationToken))
                return UpdateTicketStatus.UserNotFound;

            // The only editable field for now is the owner. Add Title/Status/etc. here once they exist.
            // Messages and attachments are changed through their own endpoints.
            ticket.UserId = command.Dto.UserId;
            await context.SaveChangesAsync(cancellationToken);

            return UpdateTicketStatus.Updated;
        }
    }
}
