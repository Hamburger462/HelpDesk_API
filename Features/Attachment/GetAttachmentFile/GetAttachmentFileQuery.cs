using HelpDesk.Data;
using HelpDesk.Services.Storage;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Features.Attachment.GetAttachmentFile
{
    public record AttachmentFile(Stream Content, string ContentType, string FileName);

    // Returns null when the attachment (or its file) does not exist on that ticket.
    public record GetAttachmentFileQuery(Guid TicketId, Guid Id) : IRequest<AttachmentFile?>;

    public class GetAttachmentFileHandler(AppDbContext context, IFileStorage storage)
        : IRequestHandler<GetAttachmentFileQuery, AttachmentFile?>
    {
        public async Task<AttachmentFile?> Handle(GetAttachmentFileQuery query, CancellationToken cancellationToken)
        {
            var attachment = await context.Attachments
                .Where(a => a.Id == query.Id && a.TicketId == query.TicketId)
                .Select(a => new { a.StoragePath, a.ContentType, a.FileName })
                .FirstOrDefaultAsync(cancellationToken);
            if (attachment is null) return null;

            var stream = await storage.OpenReadAsync(attachment.StoragePath, cancellationToken);
            return stream is null ? null : new AttachmentFile(stream, attachment.ContentType, attachment.FileName);
        }
    }
}