using AutoMapper;
using AutoMapper.QueryableExtensions;
using HelpDesk.Data;
using HelpDesk.Data.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Features.Ticket.GetTicketById
{
    public record GetTicketByIdQuery(Guid Id) : IRequest<TicketDto?>;

    public class GetTicketByIdHandler(AppDbContext context, IMapper mapper) : IRequestHandler<GetTicketByIdQuery, TicketDto?>
    {
        public async Task<TicketDto?> Handle(GetTicketByIdQuery query, CancellationToken cancellationToken)
        {
            return await context.Tickets
                .Where(t => t.Id == query.Id)
                .ProjectTo<TicketDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
