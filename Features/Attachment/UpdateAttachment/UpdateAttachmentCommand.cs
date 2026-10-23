using HelpDesk.Data;
using HelpDesk.Data.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Features.Attachment.UpdateAttachment
{
    // Only the display name can change; the stored file itself is immutable.
    // Returns false when the attachment does not exist on that ticket.
    public record UpdateAttachmentCommand(Guid TicketId, Guid Id, UpdateAttachmentDto Dto) : IRequest<bool>;

    public class UpdateAttachmentHandler(AppDbContext context) : IRequestHandler<UpdateAttachmentCommand, bool>
    {
        public async Task<bool> Handle(UpdateAttachmentCommand command, CancellationToken cancellationToken)
        {
            var attachment = await context.Attachments
                .FirstOrDefaultAsync(a => a.Id == command.Id && a.TicketId == command.TicketId, cancellationToken);
            if (attachment is null) return false;

            attachment.FileName = Path.GetFileName(command.Dto.FileName);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
