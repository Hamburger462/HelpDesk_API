using AutoMapper;
using AutoMapper.QueryableExtensions;
using HelpDesk.Data;
using HelpDesk.Data.Dto;
using HelpDesk.Services.Storage;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AttachmentEntity = HelpDesk.Data.Models.Attachment;

namespace HelpDesk.Features.Attachment.CreateAttachment
{
    // Returns null when the ticket does not exist.
    // The controller validates size/type and decides ContentType; the handler only stores.
    public record CreateAttachmentCommand(Guid TicketId, Guid UploadedById, string FileName, string ContentType, long SizeBytes, Stream Content)
        : IRequest<AttachmentDto?>;

    public class CreateAttachmentHandler(AppDbContext context, IMapper mapper, IFileStorage storage)
        : IRequestHandler<CreateAttachmentCommand, AttachmentDto?>
    {
        public async Task<AttachmentDto?> Handle(CreateAttachmentCommand command, CancellationToken cancellationToken)
        {
            if (!await context.Tickets.AnyAsync(t => t.Id == command.TicketId, cancellationToken))
                return null;

            var id = Guid.NewGuid();
            var extension = Path.GetExtension(command.FileName).ToLowerInvariant();
            var path = $"tickets/{command.TicketId}/{id}{extension}";   // never the original name

            // Upload first, then insert the row, so a failure leaves an orphan file rather than a dangling row.
            await storage.UploadAsync(path, command.Content, command.ContentType, cancellationToken);

            var attachment = new AttachmentEntity
            {
                Id = id,
                TicketId = command.TicketId,
                UploadedById = command.UploadedById,
                FileName = Path.GetFileName(command.FileName),
                ContentType = command.ContentType,
                SizeBytes = command.SizeBytes,
                StoragePath = path
            };

            try
            {
                context.Attachments.Add(attachment);
                await context.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                await storage.DeleteAsync([path], CancellationToken.None);
                throw;
            }

            // Reload with the uploader so UploadedByFullName is filled in the response.
            return await context.Attachments
                .Where(a => a.Id == id)
                .ProjectTo<AttachmentDto>(mapper.ConfigurationProvider)
                .FirstAsync(cancellationToken);
        }
    }
}