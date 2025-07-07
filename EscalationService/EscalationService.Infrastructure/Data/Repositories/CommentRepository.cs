using EscalationService.Appliacation.Common.Interfaces.Repositories;
using EscalationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EscalationService.Infrastructure.Data.Repositories;

public class CommentRepository(ApplicationDbContext db) : ICommentRepository
{
    private readonly ApplicationDbContext _db = db;

    public async Task<IEnumerable<Comment>> GetByEscalationIdAsync(int escalationId)
    {
        return await _db.Comments
            .AsNoTracking()
            .Where(c => c.EscalationId == escalationId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comment>> GetByUserIdAsync(int userId)
    {
        return await _db.Comments
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    public async Task<Comment?> GetByIdAsync(int id)
    {
        return await _db.Comments.FindAsync(id);
    }

    public async Task AddAsync(Comment comment)
    {
        await _db.Comments.AddAsync(comment);
    }

    public async Task UpdateAsync(Comment comment)
    {
        _db.Comments.Update(comment);
    }

    public async Task DeleteAsync(Comment comment)
    {
        _db.Comments.Remove(comment);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _db.Comments
            .AsNoTracking()
            .AnyAsync(c => c.Id == id);
    }
}