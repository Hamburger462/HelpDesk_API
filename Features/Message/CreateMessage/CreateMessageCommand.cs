using AutoMapper;
using HelpDesk.Data;
using HelpDesk.Data.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MessageEntity = HelpDesk.Data.Models.Message;

namespace HelpDesk.Features.Message.CreateMessage
{
    // Returns null when the ticket does not exist.
    public record CreateMessageCommand(Guid TicketId, MessageDto Dto) : IRequest<MessageDto?>;

    public class CreateMessageHandler(AppDbContext context, IMapper mapper) : IRequestHandler<CreateMessageCommand, MessageDto?>
    {
        public async Task<MessageDto?> Handle(CreateMessageCommand command, CancellationToken cancellationToken)
        {
            if (!await context.Tickets.AnyAsync(t => t.Id == command.TicketId, cancellationToken))
                return null;

            var message = mapper.Map<MessageEntity>(command.Dto);
            message.TicketId = command.TicketId;

            context.Messages.Add(message);
            await context.SaveChangesAsync(cancellationToken);

            return mapper.Map<MessageDto>(message);
        }
    }
}
