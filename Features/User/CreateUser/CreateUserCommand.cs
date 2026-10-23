using MediatR;

using HelpDesk.Data.Dto;

namespace HelpDesk.Features.User.CreateUser
{
    public record CreateUserCommand(CreateUserDto CreateUserDto): IRequest<Guid>;
}
