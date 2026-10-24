using HelpDesk.Data;
using HelpDesk.Data.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Features.Attachment.UpdateAttachment
{
    public enum UpdateAttachmentStatus { Updated, NotFound, InvalidName }

    // Only the display name can change; the stored file itself is immutable.
    // The stored file's extension is always kept, so a rename can't disguise the file type
    // (e.g. renaming a PDF to "report.exe" results in "report.exe.pdf").
    public record UpdateAttachmentCommand(Guid TicketId, Guid Id, UpdateAttachmentDto Dto) : IRequest<UpdateAttachmentStatus>;

    public class UpdateAttachmentHandler(AppDbContext context) : IRequestHandler<UpdateAttachmentCommand, UpdateAttachmentStatus>
    {
        private const int MaxFileNameLength = 255;

        public async Task<UpdateAttachmentStatus> Handle(UpdateAttachmentCommand command, CancellationToken cancellationToken)
        {
            var attachment = await context.Attachments
                .FirstOrDefaultAsync(a => a.Id == command.Id && a.TicketId == command.TicketId, cancellationToken);
            if (attachment is null) return UpdateAttachmentStatus.NotFound;

            // The stored path was built by the server from an allowlisted extension, so it can be trusted.
            var extension = Path.GetExtension(attachment.StoragePath);

            var name = Path.GetFileName(command.Dto.FileName).Trim();
            if (!name.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                name += extension;

            // Reject names that are empty once the extension is removed, or too long for the column.
            if (name.Length == extension.Length || name.Length > MaxFileNameLength)
                return UpdateAttachmentStatus.InvalidName;

            attachment.FileName = name;
            await context.SaveChangesAsync(cancellationToken);

            return UpdateAttachmentStatus.Updated;
        }
    }
}