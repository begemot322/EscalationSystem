using Models.DTOs;
using Models.QueryParams;
using Models.Result;
using UserService.Application.Common.Interfaces.Repository;
using UserService.Application.Filters;
using UserService.Application.Services.Interfaces;
using UserService.Domain;

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
        return users.Select(u => new UserDto(u.Id,u.FirstName, u.LastName, u.Email, u.PhoneNumber,u.Role)).ToList();
    }
    
    public async Task<Result<UserDto>> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return Result<UserDto>.Failure(
                Error.NotFound<User>(userId));
        }

        var userDto = new UserDto(user.Id, user.FirstName, user.LastName, user.Email, user.PhoneNumber, user.Role);

        return Result<UserDto>.Success(userDto);
    }
    
    public async Task<Result<PagedResult<User>>> GetAllUsersAsync(UserFilter? filter = null, PageParams? pageParams = null)
    {
        var users = await _userRepository.GetAllAsync(filter, pageParams);
        
        return Result<PagedResult<User>>.Success(users);
    }
}