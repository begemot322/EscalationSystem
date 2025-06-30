using Models.Result;
using UserService.Application.DTOs;
using UserService.Domain;

namespace UserService.Application.Services.Interfaces;

public interface IAuthService
{
    Task<Result<User>> RegisterAsync(RegisterUserDto dto);
    Task<Result<string>> LoginAsync(LoginUserDto dto);
}