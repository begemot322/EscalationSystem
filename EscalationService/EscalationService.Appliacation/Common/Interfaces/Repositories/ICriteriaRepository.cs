using EscalationService.Domain.Entities;

namespace EscalationService.Appliacation.Common.Interfaces.Repositories;

public interface ICriteriaRepository
{
    Task<IEnumerable<Criteria>> GetByEscalationIdAsync(int escalationId);
    Task<int> CountByEscalationIdAsync(int escalationId);
    Task<Criteria?> GetByIdAsync(int id);
    Task AddAsync(Criteria criteria);
    Task UpdateAsync(Criteria criteria);
    Task DeleteAsync(Criteria criteria);
    Task<bool> ExistsAsync(int id);
}