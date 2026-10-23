using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using HelpDesk.Data;
using HelpDesk.Data.Dto;

namespace HelpDesk.Features.User.GetAllUsers
{
    public class GetAllUsersHandlers(AppDbContext context, IMapper mapper) : IRequestHandler<GetAllUsersCommand, List<UserDto>>
    {
        public async Task<List<UserDto>> Handle(GetAllUsersCommand command, CancellationToken cancellationToken)
        {
            var users = await context.Users.ProjectTo<UserDto>(mapper.ConfigurationProvider).ToListAsync();
            return users;
        }
    }
}
