using Models.QueryParams;
using UserService.Application.Filters;
using UserService.Domain;

namespace UserService.Application.Common.Interfaces.Repository;

public interface IUserRepository
{
    Task<PagedResult<User>> GetAllAsync(UserFilter? filter, PageParams? pageParams);
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByEmailAsync(string email);
}