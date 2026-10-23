using AutoMapper;
using MediatR;

using HelpDesk.Data;
using HelpDesk.Data.Dto;
using HelpDesk.Services.Token;

namespace HelpDesk.Features.Auth.Register
{
    public class RegisterHandler(AppDbContext context, IMapper mapper, IConfiguration configuration): IRequestHandler<RegisterCommand, string>
    {
        public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var userDto = mapper.Map<CreateUserDto>(request.RegisterDto);
            userDto.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            var user = mapper.Map<Data.Models.User>(userDto);
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            var outputDto = mapper.Map<UserDto>(user);
            var token = TokenService.GenerateToken(outputDto, configuration);

            return token;
        }
    }
}
