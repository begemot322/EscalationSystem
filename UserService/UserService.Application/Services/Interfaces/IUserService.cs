using Models;
using Models.DTOs;

namespace UserService.Application.Services.Interfaces;

public interface IUserService
{
    Task<bool> CheckUsersExist(List<int> userIds);
    Task<List<UserDto>> GetUsersInfo(List<int> userIds);
}