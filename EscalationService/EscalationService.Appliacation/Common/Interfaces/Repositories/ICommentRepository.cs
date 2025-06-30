using EscalationService.Domain.Entities;

namespace EscalationService.Appliacation.Common.Interfaces.Repositories;

public interface ICommentRepository
{
    Task<IEnumerable<Comment>> GetByEscalationIdAsync(int escalationId);
    Task<IEnumerable<Comment>> GetByUserIdAsync(int userId);
    Task<Comment?> GetByIdAsync(int id);
    Task AddAsync(Comment comment);
    Task UpdateAsync(Comment comment);
    Task DeleteAsync(Comment comment);
    Task<bool> ExistsAsync(int id);
}