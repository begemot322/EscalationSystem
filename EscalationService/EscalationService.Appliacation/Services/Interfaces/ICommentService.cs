using EscalationService.Appliacation.DTOs;
using EscalationService.Domain.Entities;
using Models.Result;

namespace EscalationService.Appliacation.Services.Interfaces;

public interface ICommentService
{
    Task<Result<IEnumerable<Comment>>> GetByEscalationIdAsync(int escalationId);
    Task<Result<IEnumerable<Comment>>> GetByUserIdAsync(int userId);
    Task<Result<Comment>> CreateAsync(CommentDto dto, int escalationId, int userId);
    Task<Result<Comment>> UpdateAsync(int commentId, CommentDto dto, int userId);
    Task<Result> DeleteAsync(int commentId, int userId);
}