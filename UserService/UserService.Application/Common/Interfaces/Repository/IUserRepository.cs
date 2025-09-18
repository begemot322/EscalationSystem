using Models.QueryParams;
using UserService.Application.Models.Filters;
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
    Task<List<int>> CheckUsersExistAsync(List<int> userIds);
    Task<IEnumerable<User>> GetUsersByIdsAsync(List<int> userIds);
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByEmailAsync(string email);
}