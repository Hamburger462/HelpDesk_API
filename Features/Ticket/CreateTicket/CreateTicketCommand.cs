using AutoMapper;
using HelpDesk.Data;
using HelpDesk.Data.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketEntity = HelpDesk.Data.Models.Ticket;

namespace HelpDesk.Features.Ticket.CreateTicket
{
    // Returns null when the owning user does not exist.
    // Messages in the DTO are created with the ticket; attachments are not (they go through the upload endpoint).
    public record CreateTicketCommand(TicketDto Dto) : IRequest<TicketDto?>;

    public class CreateTicketHandler(AppDbContext context, IMapper mapper) : IRequestHandler<CreateTicketCommand, TicketDto?>
    {
        public async Task<TicketDto?> Handle(CreateTicketCommand command, CancellationToken cancellationToken)
        {
            if (!await context.Users.AnyAsync(u => u.Id == command.Dto.UserId, cancellationToken))
                return null;

            var ticket = mapper.Map<TicketEntity>(command.Dto);

            context.Tickets.Add(ticket);
            await context.SaveChangesAsync(cancellationToken);

            return mapper.Map<TicketDto>(ticket);
        }
    }
}
