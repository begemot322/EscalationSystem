using EscalationService.Appliacation.Common.Interfaces.Repositories;

namespace EscalationService.Appliacation.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ICommentRepository Comments { get; }
    ICriteriaRepository Criterias { get; }
    IEscalationRepository Escalations { get; }
    IEscalationUserRepository EscalationUsers { get; }
    
    Task<int> SaveChangesAsync();
}