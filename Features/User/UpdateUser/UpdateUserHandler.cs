using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

using HelpDesk.Data;
using HelpDesk.Data.Dto;

namespace HelpDesk.Features.User.UpdateUser
{
    public class UpdateUserHandler(AppDbContext context, IMapper mapper) : IRequestHandler<UpdateUserCommand, UserDto>
    {
        public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var userId = request.id;
            var userDto = request.UpdateUserDto;
            if (userDto.Password != null)
            {
                userDto.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            }

            var user = await context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

            user = mapper.Map<Data.Models.User>(request.UpdateUserDto);

            var outputDto = mapper.Map<UserDto>(user);

            await context.SaveChangesAsync();

            return outputDto;
        }
    }
}
