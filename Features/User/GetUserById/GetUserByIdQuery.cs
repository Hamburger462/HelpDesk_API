using MediatR;

using HelpDesk.Data.Dto;

namespace HelpDesk.Features.User.GetUserById
{
    public record GetUserByIdQuery(Guid id) : IRequest<UserDto>;
}
