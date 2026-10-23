using HelpDesk.Data;
using HelpDesk.Services.Storage;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Features.Attachment.DeleteAttachment
{
    // Returns false when the attachment does not exist on that ticket.
    public record DeleteAttachmentCommand(Guid TicketId, Guid Id) : IRequest<bool>;

    public class DeleteAttachmentHandler(AppDbContext context, IFileStorage storage) : IRequestHandler<DeleteAttachmentCommand, bool>
    {
        public async Task<bool> Handle(DeleteAttachmentCommand command, CancellationToken cancellationToken)
        {
            var attachment = await context.Attachments
                .FirstOrDefaultAsync(a => a.Id == command.Id && a.TicketId == command.TicketId, cancellationToken);
            if (attachment is null) return false;

            context.Attachments.Remove(attachment);
            await context.SaveChangesAsync(cancellationToken);

            await storage.DeleteAsync([attachment.StoragePath], cancellationToken);
            return true;
        }
    }
}
