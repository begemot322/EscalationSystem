using EscalationService.Appliacation.Common.Interfaces.Repositories;
using EscalationService.Appliacation.Filters;
using EscalationService.Domain.Entities;
using EscalationService.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.QueryParams;

namespace EscalationService.Infrastructure.Data.Repositories;

public class EscalationRepository(ApplicationDbContext db) : IEscalationRepository
{
    private readonly ApplicationDbContext _db = db;

    public async Task<PagedResult<Escalation>> GetAllAsync(EscalationFilter? filter, SortParams? sortParams, PageParams? pageParams)
    {
        return await _db.Escalations
            .AsNoTracking()
            .Filter(filter)
            .Sort(sortParams)
            .ToPageAsync(pageParams);
    }

    public async Task<Escalation?> GetByIdAsync(int id)
    {
        return await _db.Escalations.FindAsync(id);    }

    public async Task AddAsync(Escalation escalation)
    {
        await _db.Escalations.AddAsync(escalation);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Escalation escalation)
    {
        _db.Escalations.Update(escalation);
        await _db.SaveChangesAsync();;
    }

    public async Task DeleteAsync(Escalation escalation)
    {
        _db.Escalations.Remove(escalation);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _db.Escalations
            .AsNoTracking()
            .AnyAsync(e => e.Id == id);
    }
    
    public async Task<List<Escalation>> GetFilteredEscalationsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        EscalationStatus? status = null)
    {
        var query = _db.Escalations.AsNoTracking();

        if (fromDate.HasValue)
            query = query.Where(e => e.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(e => e.CreatedAt <= toDate.Value);

        if (status.HasValue)
            query = query.Where(e => e.Status == status.Value);

        return await query
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }
}