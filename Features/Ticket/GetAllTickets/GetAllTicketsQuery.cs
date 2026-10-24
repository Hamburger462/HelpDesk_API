using AutoMapper;
using AutoMapper.QueryableExtensions;
using HelpDesk.Data;
using HelpDesk.Data.Dto;
using HelpDesk.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Features.Ticket.GetAllTickets
{
    // ViewAll is true for operators and admins. Everyone else only ever sees tickets they sent,
    // whatever the filter says.
    public record GetAllTicketsQuery(TicketFilter Filter, Guid ViewerId, bool ViewAll)
        : IRequest<PagedResult<TicketSummaryDto>>;

    public class GetAllTicketsHandler(AppDbContext context, IMapper mapper)
        : IRequestHandler<GetAllTicketsQuery, PagedResult<TicketSummaryDto>>
    {
        public async Task<PagedResult<TicketSummaryDto>> Handle(GetAllTicketsQuery query, CancellationToken cancellationToken)
        {
            var filter = query.Filter;
            var tickets = context.Tickets.AsNoTracking();

            if (!query.ViewAll)
                tickets = tickets.Where(t => t.CreatedById == query.ViewerId);

            if (filter.Status is { } status)
                tickets = tickets.Where(t => t.Status == status);

            if (filter.Urgency is { } urgency)
                tickets = tickets.Where(t => t.Urgency == urgency);

            if (filter.CreatedById is { } createdById)
                tickets = tickets.Where(t => t.CreatedById == createdById);

            if (filter.AssigneeId is { } assigneeId)
                tickets = tickets.Where(t => t.Assignees.Any(a => a.Id == assigneeId));

            if (filter.Unassigned == true)
                tickets = tickets.Where(t => !t.Assignees.Any());

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim().ToLower();
                tickets = tickets.Where(t => t.Title.ToLower().Contains(search));
            }

            if (filter.CreatedFrom is { } createdFrom)
                tickets = tickets.Where(t => t.CreatedAt >= createdFrom);

            if (filter.CreatedTo is { } createdTo)
                tickets = tickets.Where(t => t.CreatedAt <= createdTo);

            return await tickets
                .OrderByDescending(t => t.CreatedAt).ThenBy(t => t.Id)
                .ToPagedResultAsync(filter.Page, filter.PageSize,
                    q => q.ProjectTo<TicketSummaryDto>(mapper.ConfigurationProvider), cancellationToken);
        }
    }
}
