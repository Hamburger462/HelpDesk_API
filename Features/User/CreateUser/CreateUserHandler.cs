using MediatR;
using AutoMapper;

using HelpDesk.Data;

namespace HelpDesk.Features.User.CreateUser
{
    public class CreateUserHandler(AppDbContext context, IMapper mapper) : IRequestHandler<CreateUserCommand, Guid>
    {
        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var userDto = request.CreateUserDto;
            userDto.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            var user = mapper.Map<Data.Models.User>(userDto);
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            return user.Id;
        }
    }
}
