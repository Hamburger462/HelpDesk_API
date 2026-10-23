using AutoMapper;
using AutoMapper.QueryableExtensions;
using HelpDesk.Data;
using HelpDesk.Data.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Features.Attachment.GetAttachmentById
{
    // Returns metadata only; use the download query for the file itself.
    public record GetAttachmentByIdQuery(Guid TicketId, Guid Id) : IRequest<AttachmentDto?>;

    public class GetAttachmentByIdHandler(AppDbContext context, IMapper mapper) : IRequestHandler<GetAttachmentByIdQuery, AttachmentDto?>
    {
        public async Task<AttachmentDto?> Handle(GetAttachmentByIdQuery query, CancellationToken cancellationToken)
        {
            return await context.Attachments
                .Where(a => a.Id == query.Id && a.TicketId == query.TicketId)
                .ProjectTo<AttachmentDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
