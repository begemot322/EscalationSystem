using FluentValidation;
using Models.Result;
using UserService.Application.Common.Interfaces.Identity;
using UserService.Application.Common.Interfaces.Repository;
using UserService.Application.Models.DTOs;
using UserService.Application.Services.Interfaces;
using UserService.Domain;

namespace UserService.Application.Services.Implementation;

public class AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider,
    IValidator<RegisterUserDto> registerValidator,
    IValidator<LoginUserDto> loginValidator) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IValidator<RegisterUserDto> _registerValidator = registerValidator;
    private readonly IValidator<LoginUserDto> _loginValidator = loginValidator;

    public async Task<Result<User>> RegisterAsync(RegisterUserDto dto)
    {
        var validationResult = await _registerValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return Result<User>.Failure(
                Error.ValidationFailed(string.Join(", ", validationResult.Errors)));
        }
        
        if (await _userRepository.ExistsByEmailAsync(dto.Email))
        {
            return Result<User>.Failure(
                Error.Duplicate("Email",dto.Email));
        }
        var hashedPassword = _passwordHasher.Generate(dto.Password);

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            PasswordHash = hashedPassword,
            Role = dto.Role
        };
        
        await _userRepository.AddAsync(user);
        return Result<User>.Success(user);
    }

    public async Task<Result<string>> LoginAsync(LoginUserDto dto)
    {
        var validationResult = await _loginValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return Result<string>.Failure(
                Error.ValidationFailed(string.Join(", ", validationResult.Errors)));
        }
        
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null)
        {
            return Result<string>.Failure(
                Error.Unauthorized("Invalid email or password"));
        }
        
        if (!_passwordHasher.Verify(dto.Password, user.PasswordHash))
        {
            return Result<string>.Failure(
                Error.Unauthorized("Invalid email or password"));
        }
        
        var token = _jwtProvider.GenerateToken(user);
        return Result<string>.Success(token);
    }
}