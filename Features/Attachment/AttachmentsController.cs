using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

using HelpDesk.Data.Dto;
using HelpDesk.Features.Attachment.GetAttachmentById;
using HelpDesk.Features.Attachment.GetAttachmentFile;
using HelpDesk.Features.Attachment.CreateAttachment;
using HelpDesk.Features.Attachment.UpdateAttachment;
using HelpDesk.Features.Attachment.DeleteAttachment;

namespace HelpDesk.Features.Attachment
{
    [Route("api/tickets/{ticketId:guid}/attachments")]
    [ApiController]
    public class AttachmentsController(IMediator mediator) : ControllerBase
    {
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".txt", ".csv", ".docx", ".xlsx", ".pdf", ".png", ".jpg", ".jpeg", ".gif", ".webp"
        };

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AttachmentDto>> GetAttachmentById(Guid ticketId, Guid id, CancellationToken cancellationToken)
        {
            var attachment = await mediator.Send(new GetAttachmentByIdQuery(ticketId, id), cancellationToken);
            return attachment is null ? NotFound() : Ok(attachment);
        }

        // Streams the file. Passing a download name makes the response "Content-Disposition: attachment".
        [HttpGet("{id:guid}/download")]
        public async Task<IActionResult> DownloadAttachment(Guid ticketId, Guid id, CancellationToken cancellationToken)
        {
            var file = await mediator.Send(new GetAttachmentFileQuery(ticketId, id), cancellationToken);
            return file is null ? NotFound() : File(file.Content, file.ContentType, file.FileName);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<AttachmentDto>> CreateAttachment(Guid ticketId, IFormFile file, CancellationToken cancellationToken)
        {
            if (file.Length == 0)
                return BadRequest(new { error = "File is empty." });
            if (file.Length > MaxFileSizeBytes)
                return BadRequest(new { error = $"File exceeds {MaxFileSizeBytes / (1024 * 1024)} MB." });
            if (file.FileName.Length > 255)
                return BadRequest(new { error = "File name is too long." });

            // The client-supplied Content-Type is ignored; the type comes from the allowlisted extension.
            if (!AllowedExtensions.Contains(Path.GetExtension(file.FileName))
                || !new FileExtensionContentTypeProvider().TryGetContentType(file.FileName, out var contentType))
                return BadRequest(new { error = "File type is not allowed." });

            await using var stream = file.OpenReadStream();
            var attachment = await mediator.Send(
                new CreateAttachmentCommand(ticketId, file.FileName, contentType, file.Length, stream), cancellationToken);

            return attachment is null
                ? NotFound(new { error = "Ticket does not exist." })
                : Created($"/api/tickets/{ticketId}/attachments/{attachment.Id}", attachment);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAttachment(Guid ticketId, Guid id, [FromBody] UpdateAttachmentDto attachmentDto, CancellationToken cancellationToken)
        {
            var updated = await mediator.Send(new UpdateAttachmentCommand(ticketId, id, attachmentDto), cancellationToken);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAttachment(Guid ticketId, Guid id, CancellationToken cancellationToken)
        {
            var deleted = await mediator.Send(new DeleteAttachmentCommand(ticketId, id), cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}
