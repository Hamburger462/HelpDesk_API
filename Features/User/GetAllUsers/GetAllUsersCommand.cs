using MediatR;
using HelpDesk.Data.Dto;

namespace HelpDesk.Features.User.GetAllUsers
{
    public record GetAllUsersCommand() : IRequest<List<UserDto>>;
}