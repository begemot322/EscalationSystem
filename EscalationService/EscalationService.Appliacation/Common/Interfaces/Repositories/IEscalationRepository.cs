using System.Linq.Expressions;
using EscalationService.Appliacation.Filters;
using EscalationService.Domain.Entities;
using Models;
using Models.QueryParams;

namespace EscalationService.Appliacation.Common.Interfaces.Repositories;

public interface IEscalationRepository
{
    Task<PagedResult<Escalation>> GetAllAsync(EscalationFilter? filter,
        SortParams? sortParams, PageParams? pageParams);
    
    Task<Escalation?> GetByIdAsync(int id);
    Task AddAsync(Escalation escalation);
    Task UpdateAsync(Escalation escalation);
    Task DeleteAsync(Escalation escalation);
    Task<bool> ExistsAsync(int id);
    Task<IEnumerable<Escalation>> GetFeaturedEscalationsAsync(int count);
    Task<IEnumerable<Escalation>> GetByExpressionAsync(
        Expression<Func<Escalation, bool>> expression,
        string? includeProperties = null,
        bool asNoTracking = true);
    
    Task<IEnumerable<Escalation>> GetFilteredEscalationsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        EscalationStatus? status = null);
}