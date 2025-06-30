using Microsoft.EntityFrameworkCore;
using Models.QueryParams;
using UserService.Application.Filters;
using UserService.Domain;

namespace UserService.Infrastructure.Extensions;

public static class UserExtensions
{
    public static IQueryable<User> Filter(this IQueryable<User> query, UserFilter? filter)
    {
        if (filter == null) return query;

        if (!string.IsNullOrEmpty(filter.FirstName))
            query = query.Where(u => u.FirstName.Contains(filter.FirstName));

        if (!string.IsNullOrEmpty(filter.LastName))
            query = query.Where(u => u.LastName.Contains(filter.LastName));

        if (!string.IsNullOrEmpty(filter.Email))
            query = query.Where(u => u.Email.Contains(filter.Email));

        if (!string.IsNullOrEmpty(filter.PhoneNumber))
            query = query.Where(u => u.PhoneNumber.Contains(filter.PhoneNumber));

        return query;
    }

    public static async Task<PagedResult<User>> ToPageAsync(this IQueryable<User> query, PageParams? pageParams)
    {
        var page = pageParams?.Page ?? 1;
        var pageSize = pageParams?.PageSize ?? 10;
        
        var totalCount = await query.CountAsync();
        
        if (totalCount == 0)
            return new PagedResult<User>(new List<User>(), totalCount, page, pageSize);

        var skip = (page - 1) * pageSize;
        var items = await query.Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<User>(items, totalCount, page, pageSize);
    }
}