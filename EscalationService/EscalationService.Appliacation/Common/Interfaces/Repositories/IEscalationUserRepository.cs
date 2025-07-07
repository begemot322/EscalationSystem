using EscalationService.Domain.Entities;

namespace EscalationService.Appliacation.Common.Interfaces.Repositories;

public interface IEscalationUserRepository
{
    Task AddAsync(EscalationUser escalationUser);
    Task AddRangeAsync(IEnumerable<EscalationUser> escalationUsers);
    Task DeleteByEscalationIdAsync(int escalationId);
}