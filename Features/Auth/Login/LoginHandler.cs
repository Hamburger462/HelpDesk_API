using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

using HelpDesk.Data;
using HelpDesk.Data.Dto;
using HelpDesk.Services.Token;

namespace HelpDesk.Features.Auth.Login
{
    public class LoginHandler(AppDbContext context, IMapper mapper, IConfiguration configuration) : IRequestHandler<LoginCommand, string?>
    {
        public async Task<string?> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var dto = request.LoginDto;
            var user = await context.Users.Where(u => u.Email == dto.Email).FirstOrDefaultAsync();
            if (user != null && BCrypt.Net.BCrypt.Verify(dto.Password, user.PassHash))
            {
                var outputDto = mapper.Map<UserDto>(user);
                var token = TokenService.GenerateToken(outputDto, configuration);
                return token;
            }
            return null;
        }
    }
}
