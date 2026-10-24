using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

using HelpDesk.Data.Dto;
using HelpDesk.Data.Models;
using HelpDesk.Extensions;
using HelpDesk.Features.Attachment.GetAllAttachments;
using HelpDesk.Features.Attachment.GetAttachmentById;
using HelpDesk.Features.Attachment.GetAttachmentFile;
using HelpDesk.Features.Attachment.CreateAttachment;
using HelpDesk.Features.Attachment.UpdateAttachment;
using HelpDesk.Features.Attachment.DeleteAttachment;

namespace HelpDesk.Features.Attachment
{
    /// <summary>
    /// Files attached to a ticket. Requires authentication.
    /// </summary>
    [Route("api/tickets/{ticketId:guid}/attachments")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class AttachmentsController(IMediator mediator) : ControllerBase
    {
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".txt", ".csv", ".docx", ".xlsx", ".pdf", ".png", ".jpg", ".jpeg", ".gif", ".webp"
        };

        /// <summary>
        /// Gets a page of a ticket's attachments (metadata only).
        /// </summary>
        /// <remarks>
        /// Oldest first. Regular users can only list attachments on tickets they sent.
        /// Use the download endpoint to get a file itself.
        /// </remarks>
        /// <param name="ticketId">The ticket id.</param>
        /// <param name="filter">Paging and filter options, sent as query-string parameters.</param>
        /// <response code="200">A page of attachment metadata.</response>
        /// <response code="404">The ticket does not exist, or the caller may not see it.</response>
        [HttpGet]
        [ProducesResponseType<PagedResult<AttachmentDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<AttachmentDto>>> GetAllAttachments(Guid ticketId, [FromQuery] AttachmentFilter filter, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            if (userId is null)
                return Unauthorized();

            var attachments = await mediator.Send(new GetAllAttachmentsQuery(ticketId, filter, userId.Value, User.IsStaff()), cancellationToken);
            return attachments is null ? NotFound() : Ok(attachments);
        }

        /// <summary>
        /// Gets an attachment's metadata.
        /// </summary>
        /// <remarks>
        /// Returns file name, type, size, uploader and creation time only. Use the download endpoint to get the file itself.
        /// </remarks>
        /// <param name="ticketId">The id of the ticket the attachment belongs to.</param>
        /// <param name="id">The attachment id.</param>
        /// <response code="200">The attachment metadata.</response>
        /// <response code="404">No such attachment exists on that ticket.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType<AttachmentDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AttachmentDto>> GetAttachmentById(Guid ticketId, Guid id, CancellationToken cancellationToken)
        {
            var attachment = await mediator.Send(new GetAttachmentByIdQuery(ticketId, id), cancellationToken);
            return attachment is null ? NotFound() : Ok(attachment);
        }

        /// <summary>
        /// Downloads an attachment's file.
        /// </summary>
        /// <remarks>
        /// The response is sent as a download (Content-Disposition: attachment) under the attachment's file name.
        /// </remarks>
        /// <param name="ticketId">The id of the ticket the attachment belongs to.</param>
        /// <param name="id">The attachment id.</param>
        /// <response code="200">The file content.</response>
        /// <response code="404">No such attachment exists on that ticket, or its file is missing from storage.</response>
        [HttpGet("{id:guid}/download")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK, "application/octet-stream")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadAttachment(Guid ticketId, Guid id, CancellationToken cancellationToken)
        {
            var file = await mediator.Send(new GetAttachmentFileQuery(ticketId, id), cancellationToken);
            return file is null ? NotFound() : File(file.Content, file.ContentType, file.FileName);
        }

        /// <summary>
        /// Uploads a file and attaches it to a ticket.
        /// </summary>
        /// <remarks>
        /// The authenticated user is recorded as the uploader.
        /// Send the request as multipart/form-data with the file in a form field named "file".
        /// The maximum size is 10 MB. Allowed types: .txt, .csv, .docx, .xlsx, .pdf, .png, .jpg, .jpeg, .gif, .webp.
        /// The content type is determined from the file extension; the one sent by the client is ignored.
        /// </remarks>
        /// <param name="ticketId">The ticket to attach the file to.</param>
        /// <param name="file">The file to upload.</param>
        /// <response code="201">The created attachment. The Location header points to it.</response>
        /// <response code="400">The file is empty, too large, has too long a name, or is not an allowed type.</response>
        /// <response code="401">The request is not from an authenticated user.</response>
        /// <response code="404">The ticket does not exist.</response>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType<AttachmentDto>(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AttachmentDto>> CreateAttachment(Guid ticketId, IFormFile file, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            if (userId is null)
                return Unauthorized();

            if (file.Length == 0)
                return Problem(detail: "File is empty.", statusCode: StatusCodes.Status400BadRequest);
            if (file.Length > MaxFileSizeBytes)
                return Problem(detail: $"File exceeds {MaxFileSizeBytes / (1024 * 1024)} MB.", statusCode: StatusCodes.Status400BadRequest);
            if (file.FileName.Length > 255)
                return Problem(detail: "File name is too long.", statusCode: StatusCodes.Status400BadRequest);

            // The client-supplied Content-Type is ignored; the type comes from the allowlisted extension.
            if (!AllowedExtensions.Contains(Path.GetExtension(file.FileName))
                || !new FileExtensionContentTypeProvider().TryGetContentType(file.FileName, out var contentType))
                return Problem(detail: "File type is not allowed.", statusCode: StatusCodes.Status400BadRequest);

            await using var stream = file.OpenReadStream();
            var attachment = await mediator.Send(
                new CreateAttachmentCommand(ticketId, userId.Value, file.FileName, contentType, file.Length, stream), cancellationToken);

            if (attachment is null)
                return Problem(detail: "Ticket does not exist.", statusCode: StatusCodes.Status404NotFound);

            return Created($"/api/tickets/{ticketId}/attachments/{attachment.Id}", attachment);
        }

        /// <summary>
        /// Renames an attachment.
        /// </summary>
        /// <remarks>
        /// Only the display name changes; the stored file is not modified.
        /// The file's original extension is always kept: it is appended if the new name lacks it.
        /// </remarks>
        /// <param name="ticketId">The id of the ticket the attachment belongs to.</param>
        /// <param name="id">The attachment id.</param>
        /// <param name="attachmentDto">The new file name (1 to 255 characters, including the extension).</param>
        /// <response code="204">The attachment was renamed.</response>
        /// <response code="400">The request body is invalid, or the name is empty or too long.</response>
        /// <response code="404">No such attachment exists on that ticket.</response>
        /// <response code="403">Requires the Operator or Admin role.</response>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = Roles.Operator + "," + Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAttachment(Guid ticketId, Guid id, [FromBody] UpdateAttachmentDto attachmentDto, CancellationToken cancellationToken)
        {
            var status = await mediator.Send(new UpdateAttachmentCommand(ticketId, id, attachmentDto), cancellationToken);
            return status switch
            {
                UpdateAttachmentStatus.Updated => NoContent(),
                UpdateAttachmentStatus.InvalidName => Problem(detail: "File name is empty or too long.", statusCode: StatusCodes.Status400BadRequest),
                _ => NotFound()
            };
        }

        /// <summary>
        /// Deletes an attachment and its stored file.
        /// </summary>
        /// <param name="ticketId">The id of the ticket the attachment belongs to.</param>
        /// <param name="id">The attachment id.</param>
        /// <response code="204">The attachment was deleted.</response>
        /// <response code="404">No such attachment exists on that ticket.</response>
        /// <response code="403">Requires the Admin role.</response>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAttachment(Guid ticketId, Guid id, CancellationToken cancellationToken)
        {
            var deleted = await mediator.Send(new DeleteAttachmentCommand(ticketId, id), cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}
