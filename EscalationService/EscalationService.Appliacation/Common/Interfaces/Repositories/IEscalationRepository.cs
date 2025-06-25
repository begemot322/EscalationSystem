using EscalationService.Appliacation.Filters;
using EscalationService.Domain.Entities;
using QueryParams;

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
}