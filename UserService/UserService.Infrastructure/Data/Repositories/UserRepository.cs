using Microsoft.EntityFrameworkCore;
using Models;
using Models.QueryParams;
using UserService.Application.Common.Interfaces.Repository;
using UserService.Application.Models.Filters;
using UserService.Domain;
using UserService.Infrastructure.Extensions;

namespace UserService.Infrastructure.Data.Repositories;

public class UserRepository(ApplicationDbContext db) : IUserRepository
{
    private readonly ApplicationDbContext _db = db;

    public async Task<PagedResult<User>> GetAllAsync(UserFilter? filter, PageParams? pageParams)
    {
        return await _db.Users
            .AsNoTracking()
            .Filter(filter)
            .ToPageAsync(pageParams);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddAsync(User user)
    {
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
    }

    public async Task<List<int>> CheckUsersExistAsync(List<int> userIds)
    {
        return await _db.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => u.Id)
            .ToListAsync();
    }
    public async Task<IEnumerable<User>> GetUsersByIdsAsync(List<int> userIds)
    {
        return await _db.Users
            .Where(u => userIds.Contains(u.Id))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _db.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == id);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _db.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == email);
    }
    
}
