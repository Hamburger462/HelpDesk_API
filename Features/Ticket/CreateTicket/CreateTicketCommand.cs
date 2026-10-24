using AutoMapper;
using AutoMapper.QueryableExtensions;
using HelpDesk.Data;
using HelpDesk.Data.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketEntity = HelpDesk.Data.Models.Ticket;

namespace HelpDesk.Features.Ticket.CreateTicket
{
    // CreatedById is the sender, taken from the authenticated user (never from the request body).
    // Returns null when that user does not exist.
    // Only Title, Urgency and the initial messages are taken from the DTO. The ticket starts as Open.
    // Attachments are not created here (they go through the upload endpoint).
    public record CreateTicketCommand(Guid CreatedById, TicketDto Dto) : IRequest<TicketDto?>;

    public class CreateTicketHandler(AppDbContext context, IMapper mapper) : IRequestHandler<CreateTicketCommand, TicketDto?>
    {
        public async Task<TicketDto?> Handle(CreateTicketCommand command, CancellationToken cancellationToken)
        {
            if (!await context.Users.AnyAsync(u => u.Id == command.CreatedById, cancellationToken))
                return null;

            var ticket = mapper.Map<TicketEntity>(command.Dto);
            ticket.Title = ticket.Title.Trim();
            ticket.CreatedById = command.CreatedById;

            // The sender wrote the initial messages.
            foreach (var message in ticket.Messages)
                message.AuthorId = command.CreatedById;

            context.Tickets.Add(ticket);
            await context.SaveChangesAsync(cancellationToken);

            // Re-read with a projection so the response includes author names.
            return await context.Tickets
                .Where(t => t.Id == ticket.Id)
                .ProjectTo<TicketDto>(mapper.ConfigurationProvider)
                .FirstAsync(cancellationToken);
        }
    }
}