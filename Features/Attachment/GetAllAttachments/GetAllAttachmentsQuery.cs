using AutoMapper;
using AutoMapper.QueryableExtensions;
using HelpDesk.Data;
using HelpDesk.Data.Dto;
using HelpDesk.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Features.Attachment.GetAllAttachments
{
    // Returns null when the ticket does not exist or the viewer may not see it
    // (regular users can only see tickets they sent).
    public record GetAllAttachmentsQuery(Guid TicketId, AttachmentFilter Filter, Guid ViewerId, bool ViewAll)
        : IRequest<PagedResult<AttachmentDto>?>;

    public class GetAllAttachmentsHandler(AppDbContext context, IMapper mapper)
        : IRequestHandler<GetAllAttachmentsQuery, PagedResult<AttachmentDto>?>
    {
        public async Task<PagedResult<AttachmentDto>?> Handle(GetAllAttachmentsQuery query, CancellationToken cancellationToken)
        {
            var filter = query.Filter;

            var ticketVisible = await context.Tickets.AnyAsync(
                t => t.Id == query.TicketId && (query.ViewAll || t.CreatedById == query.ViewerId),
                cancellationToken);
            if (!ticketVisible) return null;

            var attachments = context.Attachments.AsNoTracking().Where(a => a.TicketId == query.TicketId);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim().ToLower();
                attachments = attachments.Where(a => a.FileName.ToLower().Contains(search));
            }

            if (filter.UploadedById is { } uploadedById)
                attachments = attachments.Where(a => a.UploadedById == uploadedById);

            if (!string.IsNullOrWhiteSpace(filter.ContentType))
            {
                var contentType = filter.ContentType.Trim().ToLower();
                attachments = attachments.Where(a => a.ContentType.StartsWith(contentType));
            }

            if (filter.CreatedFrom is { } createdFrom)
                attachments = attachments.Where(a => a.CreatedAt >= createdFrom);

            if (filter.CreatedTo is { } createdTo)
                attachments = attachments.Where(a => a.CreatedAt <= createdTo);

            return await attachments
                .OrderBy(a => a.CreatedAt).ThenBy(a => a.Id)
                .ToPagedResultAsync(filter.Page, filter.PageSize,
                    q => q.ProjectTo<AttachmentDto>(mapper.ConfigurationProvider), cancellationToken);
        }
    }
}
