using HelpDesk.Data.Dto;
using MediatR;

namespace HelpDesk.Features.Auth.Register
{
    public record RegisterDto(string FullName, string Email, string Password);
    public record RegisterCommand(RegisterDto RegisterDto): IRequest<string>;
}
