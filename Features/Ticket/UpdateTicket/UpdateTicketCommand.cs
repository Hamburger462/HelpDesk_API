using HelpDesk.Data;
using HelpDesk.Data.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Features.Ticket.UpdateTicket
{
    public enum UpdateTicketStatus { Updated, NotFound }

    public record UpdateTicketCommand(Guid Id, TicketDto Dto) : IRequest<UpdateTicketStatus>;

    public class UpdateTicketHandler(AppDbContext context) : IRequestHandler<UpdateTicketCommand, UpdateTicketStatus>
    {
        public async Task<UpdateTicketStatus> Handle(UpdateTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = await context.Tickets.FirstOrDefaultAsync(t => t.Id == command.Id, cancellationToken);
            if (ticket is null) return UpdateTicketStatus.NotFound;

            // Only the title and urgency are edited here. Everything else has its own path:
            // - status: a dedicated status-change command (it enforces the workflow rules)
            // - assignees: the assign/unassign endpoints
            // - messages and attachments: their own endpoints
            // - sender (CreatedById) and timestamps: never client-controlled
            // UpdatedAt is refreshed by AppDbContext.SaveChanges.
            ticket.Title = command.Dto.Title.Trim();
            ticket.Urgency = command.Dto.Urgency;

            await context.SaveChangesAsync(cancellationToken);

            return UpdateTicketStatus.Updated;
        }
    }
}