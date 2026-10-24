using AutoMapper;
using AutoMapper.QueryableExtensions;
using HelpDesk.Data;
using HelpDesk.Data.Dto;
using HelpDesk.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Features.Message.GetAllMessages
{
    // Returns null when the ticket does not exist or the viewer may not see it
    // (regular users can only see tickets they sent).
    public record GetAllMessagesQuery(Guid TicketId, MessageFilter Filter, Guid ViewerId, bool ViewAll)
        : IRequest<PagedResult<MessageDto>?>;

    public class GetAllMessagesHandler(AppDbContext context, IMapper mapper)
        : IRequestHandler<GetAllMessagesQuery, PagedResult<MessageDto>?>
    {
        public async Task<PagedResult<MessageDto>?> Handle(GetAllMessagesQuery query, CancellationToken cancellationToken)
        {
            var filter = query.Filter;

            var ticketVisible = await context.Tickets.AnyAsync(
                t => t.Id == query.TicketId && (query.ViewAll || t.CreatedById == query.ViewerId),
                cancellationToken);
            if (!ticketVisible) return null;

            var messages = context.Messages.AsNoTracking().Where(m => m.TicketId == query.TicketId);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim().ToLower();
                messages = messages.Where(m => m.Text.ToLower().Contains(search));
            }

            if (filter.AuthorId is { } authorId)
                messages = messages.Where(m => m.AuthorId == authorId);

            if (filter.CreatedFrom is { } createdFrom)
                messages = messages.Where(m => m.CreatedAt >= createdFrom);

            if (filter.CreatedTo is { } createdTo)
                messages = messages.Where(m => m.CreatedAt <= createdTo);

            // Oldest first, so pages read as a conversation.
            return await messages
                .OrderBy(m => m.CreatedAt).ThenBy(m => m.Id)
                .ToPagedResultAsync(filter.Page, filter.PageSize,
                    q => q.ProjectTo<MessageDto>(mapper.ConfigurationProvider), cancellationToken);
        }
    }
}
