using EscalationService.Appliacation.Common.Interfaces.Repositories;
using EscalationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EscalationService.Infrastructure.Data.Repositories;

public class EscalationUserRepository : IEscalationUserRepository
{
    private readonly ApplicationDbContext _db;

    public EscalationUserRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(EscalationUser escalationUser)
    {
        await _db.EscalationUsers.AddAsync(escalationUser);
    }

    public async Task AddRangeAsync(IEnumerable<EscalationUser> escalationUsers)
    {
        await _db.EscalationUsers.AddRangeAsync(escalationUsers);
    }

    public async Task DeleteByEscalationIdAsync(int escalationId)
    {
        var entities = await _db.EscalationUsers
            .Where(eu => eu.EscalationId == escalationId)
            .ToListAsync();

        _db.EscalationUsers.RemoveRange(entities);
    }
}