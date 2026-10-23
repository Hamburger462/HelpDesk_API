using MediatR;

namespace HelpDesk.Features.User.DeleteUser
{
    public record DeleteUserCommand(Guid id): IRequest<Guid?>;
}
