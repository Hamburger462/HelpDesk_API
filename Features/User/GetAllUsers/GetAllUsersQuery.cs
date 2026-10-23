using AutoMapper;
using AutoMapper.QueryableExtensions;
using HelpDesk.Data;
using HelpDesk.Data.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Features.User.GetAllUsers
{
    public record GetAllUsersCommand() : IRequest<List<UserDto>>;

    public class GetAllUsersHandlers(AppDbContext context, IMapper mapper): IRequestHandler<GetAllUsersCommand, List<UserDto>>
    {
        public async Task<List<UserDto>> Handle(GetAllUsersCommand command, CancellationToken cancellationToken)
        {
            var users = await context.Users.ProjectTo<UserDto>(mapper.ConfigurationProvider).ToListAsync();
            return users;
        }
    }
}