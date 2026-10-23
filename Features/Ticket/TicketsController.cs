using MediatR;
using Microsoft.AspNetCore.Mvc;

using HelpDesk.Data.Dto;
using HelpDesk.Features.Ticket.GetTicketById;
using HelpDesk.Features.Ticket.CreateTicket;
using HelpDesk.Features.Ticket.UpdateTicket;
using HelpDesk.Features.Ticket.DeleteTicket;

namespace HelpDesk.Features.Ticket
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController(IMediator mediator) : ControllerBase
    {
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TicketDto>> GetTicketById(Guid id, CancellationToken cancellationToken)
        {
            var ticket = await mediator.Send(new GetTicketByIdQuery(id), cancellationToken);
            return ticket is null ? NotFound() : Ok(ticket);
        }

        [HttpPost]
        public async Task<ActionResult<TicketDto>> CreateTicket([FromBody] TicketDto ticketDto, CancellationToken cancellationToken)
        {
            var ticket = await mediator.Send(new CreateTicketCommand(ticketDto), cancellationToken);
            return ticket is null
                ? BadRequest(new { error = "User does not exist." })
                : Created($"/api/tickets/{ticket.Id}", ticket);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateTicket(Guid id, [FromBody] TicketDto ticketDto, CancellationToken cancellationToken)
        {
            var status = await mediator.Send(new UpdateTicketCommand(id, ticketDto), cancellationToken);
            return status switch
            {
                UpdateTicketStatus.Updated => NoContent(),
                UpdateTicketStatus.UserNotFound => BadRequest(new { error = "User does not exist." }),
                _ => NotFound()
            };
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTicket(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await mediator.Send(new DeleteTicketCommand(id), cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}
