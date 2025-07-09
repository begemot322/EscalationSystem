using Models;
using Models.DTOs;
using UserService.Application.Common.Interfaces.Repository;
using UserService.Application.Services.Interfaces;

namespace UserService.Application.Services.Implementation;

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<bool> CheckUsersExist(List<int> userIds)
    {
        if (userIds == null || !userIds.Any())
            return false;
        
        var existingUsersId = await _userRepository.CheckUsersExistAsync(userIds);
        return existingUsersId.Count == userIds.Count;
    }
    
    public async Task<List<UserDto>> GetUsersInfo(List<int> userIds)
    {
        var users = await _userRepository.GetUsersByIdsAsync(userIds);
        return users.Select(u => new UserDto(u.Id,u.FirstName, u.LastName, u.Email)).ToList();
    }
}