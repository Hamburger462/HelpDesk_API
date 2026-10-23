using MediatR;
using Microsoft.AspNetCore.Mvc;

using HelpDesk.Data.Dto;
using HelpDesk.Features.Message.GetMessageById;
using HelpDesk.Features.Message.CreateMessage;
using HelpDesk.Features.Message.UpdateMessage;
using HelpDesk.Features.Message.DeleteMessage;

namespace HelpDesk.Features.Message
{
    [Route("api/tickets/{ticketId:guid}/messages")]
    [ApiController]
    public class MessagesController(IMediator mediator) : ControllerBase
    {
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MessageDto>> GetMessageById(Guid ticketId, Guid id, CancellationToken cancellationToken)
        {
            var message = await mediator.Send(new GetMessageByIdQuery(ticketId, id), cancellationToken);
            return message is null ? NotFound() : Ok(message);
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(Guid ticketId, [FromBody] MessageDto messageDto, CancellationToken cancellationToken)
        {
            var message = await mediator.Send(new CreateMessageCommand(ticketId, messageDto), cancellationToken);
            return message is null
                ? NotFound(new { error = "Ticket does not exist." })
                : Created($"/api/tickets/{ticketId}/messages/{message.Id}", message);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateMessage(Guid ticketId, Guid id, [FromBody] MessageDto messageDto, CancellationToken cancellationToken)
        {
            var updated = await mediator.Send(new UpdateMessageCommand(ticketId, id, messageDto), cancellationToken);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteMessage(Guid ticketId, Guid id, CancellationToken cancellationToken)
        {
            var deleted = await mediator.Send(new DeleteMessageCommand(ticketId, id), cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}
