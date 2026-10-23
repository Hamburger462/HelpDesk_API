using HelpDesk.Data.Dto;
using MediatR;

namespace HelpDesk.Features.User.UpdateUser
{
    public record UpdateUserCommand(Guid id, UpdateUserDto UpdateUserDto): IRequest<UserDto>;
}
