using Models;
using Models.DTOs;

namespace NotificationService.API.Contracts;

public interface IUserServiceClient
{
    Task<List<UserDto>> GetUsersByIdsAsync(List<int> userIds);
}