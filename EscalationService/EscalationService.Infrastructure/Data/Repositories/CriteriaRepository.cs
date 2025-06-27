using EscalationService.Appliacation.Common.Interfaces.Repositories;
using EscalationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EscalationService.Infrastructure.Data.Repositories;

public class CriteriaRepository(ApplicationDbContext db) : ICriteriaRepository
{
    private readonly ApplicationDbContext _db = db;
    
    public async Task<List<Criteria>> GetByEscalationIdAsync(int escalationId)
    {
        return await _db.Criterias
            .AsNoTracking()
            .Where(c => c.EscalationId == escalationId)
            .OrderBy(c => c.Order)
            .ToListAsync();
    }
    
    public async Task<int> CountByEscalationIdAsync(int escalationId)
    {
        return await _db.Criterias
            .AsNoTracking()
            .CountAsync(c => c.EscalationId == escalationId);
    }

    public async Task<Criteria?> GetByIdAsync(int id)
    {
        return await _db.Criterias.FindAsync(id);
    }

    public async Task AddAsync(Criteria criteria)
    {
        await _db.Criterias.AddAsync(criteria);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Criteria criteria)
    {
        _db.Criterias.Update(criteria);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Criteria criteria)
    {
        _db.Criterias.Remove(criteria);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _db.Criterias
            .AsNoTracking()
            .AnyAsync(e => e.Id == id);
    }
}
