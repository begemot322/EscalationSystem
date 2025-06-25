using System.Linq.Expressions;
using EscalationService.Appliacation.Filters;
using EscalationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using QueryParams;

namespace EscalationService.Infrastructure.Extensions;

public static class EscalationExtension
{
    
    public static IQueryable<Escalation> Filter(this IQueryable<Escalation> query, EscalationFilter? filter)
    {
        if (filter.Status.HasValue)
            query = query.Where(e => e.Status == filter.Status.Value);
            
        if (filter.AuthorId.HasValue)
            query = query.Where(e => e.AuthorId == filter.AuthorId.Value);

        if (filter.CreatedAfter.HasValue)
            query = query.Where(e => e.CreatedAt >= filter.CreatedAfter.Value);

        if (filter.CreatedBefore.HasValue)
            query = query.Where(e => e.CreatedAt <= filter.CreatedBefore.Value);

        return query;
    }
    
    public static IQueryable<Escalation> Sort(this IQueryable<Escalation> query, SortParams? sortParams)
    {
        if (sortParams == null)
            return query.OrderByDescending(e => e.CreatedAt); 
        
        return sortParams.SortDirection == SortDirection.Descending
            ? query.OrderByDescending(GetKeySelector(sortParams.OrderBy))
            : query.OrderBy(GetKeySelector(sortParams.OrderBy));
    }

    private static Expression<Func<Escalation, object>> GetKeySelector(string? sortOrderBy)
    {
        if (string.IsNullOrEmpty(sortOrderBy))
            return x => x.CreatedAt;
        
        return sortOrderBy switch
        {
            nameof(Escalation.Name) => x => x.Name,
            nameof(Escalation.Status) => x => x.Status,
            nameof(Escalation.CreatedAt) => x => x.CreatedAt,
            nameof(Escalation.UpdatedAt) => x => x.UpdatedAt,
            nameof(Escalation.AuthorId)  => x => x.AuthorId,
            _ => x => x.CreatedAt 
        };
    }
    
    public static async Task<PagedResult<Escalation>> ToPageAsync(this IQueryable<Escalation> query, PageParams? pageParams)
    {
        var page = pageParams.Page ?? 1;
        var pageSize = pageParams.PageSize ?? 10;
        
        var totalCount = await query.CountAsync();
        
        if (totalCount == 0)
            return new PagedResult<Escalation>(new List<Escalation>(), totalCount, page, pageSize);

        var skip = (page - 1) * pageSize;
        var items  =  await query.Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Escalation>(items, totalCount, page, pageSize);
    }
}