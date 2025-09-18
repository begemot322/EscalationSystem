using EscalationService.Appliacation.Models.DTOs;
using EscalationService.Domain.Entities;
using Models.Result;

namespace EscalationService.Appliacation.Services.Interfaces;

public interface ICommentService
{
    Task<Result<IEnumerable<Comment>>> GetByEscalationIdAsync(int escalationId);
    Task<Result<IEnumerable<Comment>>> GetByUserIdAsync(int userId);
    Task<Result<Comment>> CreateAsync(CommentDto dto, int escalationId);
    Task<Result<Comment>> UpdateAsync(int commentId, CommentDto dto);
    Task<Result> DeleteAsync(int commentId);
}