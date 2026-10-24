using HelpDesk.Data.Dto;
using HelpDesk.Data.Models;
using HelpDesk.Extensions;
using HelpDesk.Features.Ticket.CreateTicket;
using HelpDesk.Features.Ticket.DeleteTicket;
using HelpDesk.Features.Ticket.GetTicketById;
using HelpDesk.Features.Ticket.UpdateTicket;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Features.Ticket
{
    /// <summary>
    /// Support tickets.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class TicketsController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Gets a ticket by id.
        /// </summary>
        /// <param name="id">The ticket id.</param>
        /// <response code="200">The ticket, with its assignees, messages and attachment metadata.</response>
        /// <response code="404">No ticket with that id exists.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType<TicketDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TicketDto>> GetTicketById(Guid id, CancellationToken cancellationToken)
        {
            var ticket = await mediator.Send(new GetTicketByIdQuery(id), cancellationToken);
            return ticket is null ? NotFound() : Ok(ticket);
        }

        /// <summary>
        /// Creates a ticket.
        /// </summary>
        /// <remarks>
        /// The authenticated user becomes the sender. The ticket starts as Open.
        /// Only the title, urgency and initial messages are taken from the request body, and the sender is recorded as the author of those messages.
        /// Everything else in the body (ids, status, timestamps, sender, assignees, attachments) is ignored; files are added through the attachments endpoint.
        /// </remarks>
        /// <param name="ticketDto">The ticket to create.</param>
        /// <response code="201">The created ticket. The Location header points to it.</response>
        /// <response code="400">The request body is invalid.</response>
        /// <response code="401">The request is not from an authenticated user.</response>
        [HttpPost]
        [ProducesResponseType<TicketDto>(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TicketDto>> CreateTicket([FromBody] TicketDto ticketDto, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            if (userId is null)
                return Unauthorized();

            var ticket = await mediator.Send(new CreateTicketCommand(userId.Value, ticketDto), cancellationToken);
            if (ticket is null)
                return Unauthorized();   // the token refers to a user that no longer exists

            return Created($"/api/tickets/{ticket.Id}", ticket);
        }

        /// <summary>
        /// Updates a ticket.
        /// </summary>
        /// <remarks>
        /// Only the title and urgency are changed. Status, assignees, messages and attachments are managed through their own endpoints.
        /// </remarks>
        /// <param name="id">The ticket id.</param>
        /// <param name="ticketDto">The new ticket values.</param>
        /// <response code="204">The ticket was updated.</response>
        /// <response code="400">The request body is invalid.</response>
        /// <response code="404">No ticket with that id exists.</response>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = Roles.Operator + "," + Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTicket(Guid id, [FromBody] TicketDto ticketDto, CancellationToken cancellationToken)
        {
            var status = await mediator.Send(new UpdateTicketCommand(id, ticketDto), cancellationToken);
            return status switch
            {
                UpdateTicketStatus.Updated => NoContent(),
                _ => NotFound()
            };
        }

        /// <summary>
        /// Deletes a ticket.
        /// </summary>
        /// <remarks>
        /// Also deletes the ticket's messages and attachments, including the stored files.
        /// </remarks>
        /// <param name="id">The ticket id.</param>
        /// <response code="204">The ticket was deleted.</response>
        /// <response code="404">No ticket with that id exists.</response>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTicket(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await mediator.Send(new DeleteTicketCommand(id), cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}