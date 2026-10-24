using AutoMapper;
using AutoMapper.QueryableExtensions;
using HelpDesk.Data;
using HelpDesk.Data.Dto;
using HelpDesk.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Features.User.GetAllUsers
{
    public record GetAllUsersCommand(UserFilter Filter) : IRequest<PagedResult<UserListItemDto>>;

    public class GetAllUsersHandlers(AppDbContext context, IMapper mapper)
        : IRequestHandler<GetAllUsersCommand, PagedResult<UserListItemDto>>
    {
        public async Task<PagedResult<UserListItemDto>> Handle(GetAllUsersCommand command, CancellationToken cancellationToken)
        {
            var filter = command.Filter;
            var users = context.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim().ToLower();
                users = users.Where(u => u.FullName.ToLower().Contains(search) || u.Email.ToLower().Contains(search));
            }

            if (!string.IsNullOrEmpty(filter.Role))
                users = users.Where(u => u.Role == filter.Role);

            return await users
                .OrderBy(u => u.FullName).ThenBy(u => u.Id)
                .ToPagedResultAsync(filter.Page, filter.PageSize,
                    q => q.ProjectTo<UserListItemDto>(mapper.ConfigurationProvider), cancellationToken);
        }
    }
}
