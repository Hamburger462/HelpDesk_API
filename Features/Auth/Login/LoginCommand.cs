using MediatR;

namespace HelpDesk.Features.Auth.Login
{
    public record LoginDto(string Email, string Password);
    public record LoginCommand(LoginDto LoginDto): IRequest<string?>;
}
