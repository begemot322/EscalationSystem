using Models;
using Models.DTOs;
using Models.QueryParams;
using Models.Result;
using UserService.Application.Models.Filters;
using UserService.Domain;

namespace UserService.Application.Services.Interfaces;

public interface IUserService
{
    Task<bool> CheckUsersExist(List<int> userIds);
    Task<List<UserDto>> GetUsersInfo(List<int> userIds);
    Task<Result<UserDto>> GetUserByIdAsync(int userId);

    Task<Result<PagedResult<User>>> GetAllUsersAsync(UserFilter? filter = null, PageParams? pageParams = null);
}