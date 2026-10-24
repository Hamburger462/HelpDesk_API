using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using HelpDesk.Data.Dto;
using HelpDesk.Data.Models;
using HelpDesk.Extensions;
using HelpDesk.Features.Message.GetAllMessages;
using HelpDesk.Features.Message.GetMessageById;
using HelpDesk.Features.Message.CreateMessage;
using HelpDesk.Features.Message.UpdateMessage;
using HelpDesk.Features.Message.DeleteMessage;

namespace HelpDesk.Features.Message
{
    /// <summary>
    /// Messages in a ticket's conversation. Requires authentication.
    /// </summary>
    [Route("api/tickets/{ticketId:guid}/messages")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class MessagesController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Gets a page of a ticket's messages.
        /// </summary>
        /// <remarks>
        /// Oldest first, so pages read as a conversation. Regular users can only read messages on tickets they sent.
        /// </remarks>
        /// <param name="ticketId">The ticket id.</param>
        /// <param name="filter">Paging and filter options, sent as query-string parameters.</param>
        /// <response code="200">A page of messages.</response>
        /// <response code="404">The ticket does not exist, or the caller may not see it.</response>
        [HttpGet]
        [ProducesResponseType<PagedResult<MessageDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<MessageDto>>> GetAllMessages(Guid ticketId, [FromQuery] MessageFilter filter, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            if (userId is null)
                return Unauthorized();

            var messages = await mediator.Send(new GetAllMessagesQuery(ticketId, filter, userId.Value, User.IsStaff()), cancellationToken);
            return messages is null ? NotFound() : Ok(messages);
        }

        /// <summary>
        /// Gets a message by id.
        /// </summary>
        /// <param name="ticketId">The id of the ticket the message belongs to.</param>
        /// <param name="id">The message id.</param>
        /// <response code="200">The message.</response>
        /// <response code="404">No such message exists on that ticket.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType<MessageDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageDto>> GetMessageById(Guid ticketId, Guid id, CancellationToken cancellationToken)
        {
            var message = await mediator.Send(new GetMessageByIdQuery(ticketId, id), cancellationToken);
            return message is null ? NotFound() : Ok(message);
        }

        /// <summary>
        /// Adds a message to a ticket.
        /// </summary>
        /// <param name="ticketId">The ticket to add the message to.</param>
        /// <param name="messageDto">The message to create. Id and timestamp are ignored.</param>
        /// <response code="201">The created message. The Location header points to it.</response>
        /// <response code="400">The request body is invalid.</response>
        /// <response code="404">The ticket does not exist.</response>
        [HttpPost]
        [ProducesResponseType<MessageDto>(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageDto>> CreateMessage(Guid ticketId, [FromBody] MessageDto messageDto, CancellationToken cancellationToken)
        {
            var message = await mediator.Send(new CreateMessageCommand(ticketId, messageDto), cancellationToken);
            if (message is null)
                return Problem(detail: "Ticket does not exist.", statusCode: StatusCodes.Status404NotFound);

            return Created($"/api/tickets/{ticketId}/messages/{message.Id}", message);
        }

        /// <summary>
        /// Edits a message's text.
        /// </summary>
        /// <param name="ticketId">The id of the ticket the message belongs to.</param>
        /// <param name="id">The message id.</param>
        /// <param name="messageDto">The new message values. Only the text is changed.</param>
        /// <response code="204">The message was updated.</response>
        /// <response code="400">The request body is invalid.</response>
        /// <response code="404">No such message exists on that ticket.</response>
        /// <response code="403">Requires the Operator or Admin role.</response>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = Roles.Operator + "," + Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMessage(Guid ticketId, Guid id, [FromBody] MessageDto messageDto, CancellationToken cancellationToken)
        {
            var updated = await mediator.Send(new UpdateMessageCommand(ticketId, id, messageDto), cancellationToken);
            return updated ? NoContent() : NotFound();
        }

        /// <summary>
        /// Deletes a message.
        /// </summary>
        /// <param name="ticketId">The id of the ticket the message belongs to.</param>
        /// <param name="id">The message id.</param>
        /// <response code="204">The message was deleted.</response>
        /// <response code="404">No such message exists on that ticket.</response>
        /// <response code="403">Requires the Admin role.</response>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMessage(Guid ticketId, Guid id, CancellationToken cancellationToken)
        {
            var deleted = await mediator.Send(new DeleteMessageCommand(ticketId, id), cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}
