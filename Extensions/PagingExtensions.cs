using HelpDesk.Data.Dto;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Extensions
{
    public static class PagingExtensions
    {
        // Counts the filtered rows, then loads one page and projects it.
        // Takes an ordered query so a page is never cut from an unordered result.
        public static async Task<PagedResult<TDto>> ToPagedResultAsync<TEntity, TDto>(
            this IOrderedQueryable<TEntity> query,
            int page,
            int pageSize,
            Func<IQueryable<TEntity>, IQueryable<TDto>> project,
            CancellationToken cancellationToken = default)
        {
            var total = await query.CountAsync(cancellationToken);
            var items = await project(query.Skip((page - 1) * pageSize).Take(pageSize))
                .ToListAsync(cancellationToken);

            return new PagedResult<TDto>(items, page, pageSize, total);
        }
    }
}
