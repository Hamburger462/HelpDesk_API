using AutoMapper;
using MediatR;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

using HelpDesk.Data;
using HelpDesk.Data.Dto;

namespace HelpDesk.Features.User.GetUserById
{
    public class GetUserByIdHandlers(AppDbContext context, IMapper mapper) : IRequestHandler<GetUserByIdQuery, UserDto?>
    {
        public async Task<UserDto?> Handle(GetUserByIdQuery command, CancellationToken cancellationToken)
        {
            var user = await context.Users.ProjectTo<UserDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(u => u.Id == command.id);
            if (user == null)
            {
                return null;
            }
            return user;
        }
    }
}
