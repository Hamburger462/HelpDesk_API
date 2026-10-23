using AutoMapper;
using AutoMapper.QueryableExtensions;
using HelpDesk.Data;
using HelpDesk.Data.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Features.Message.GetMessageById
{
    public record GetMessageByIdQuery(Guid TicketId, Guid Id) : IRequest<MessageDto?>;

    public class GetMessageByIdHandler(AppDbContext context, IMapper mapper) : IRequestHandler<GetMessageByIdQuery, MessageDto?>
    {
        public async Task<MessageDto?> Handle(GetMessageByIdQuery query, CancellationToken cancellationToken)
        {
            return await context.Messages
                .Where(m => m.Id == query.Id && m.TicketId == query.TicketId)
                .ProjectTo<MessageDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
