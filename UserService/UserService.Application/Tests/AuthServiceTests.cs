using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Models.Result;
using NSubstitute;
using UserService.Application.Common.Identity;
using UserService.Application.Common.Interfaces.Repository;
using UserService.Application.DTOs;
using UserService.Application.Services.Implementation;
using UserService.Domain;
using Xunit;

namespace UserService.Application.Tests;

public class AuthServiceTests
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly IPasswordHasher _passwordHasherMock;
    private readonly IJwtProvider _jwtProviderMock;
    private readonly IValidator<RegisterUserDto> _registerValidatorMock;
    private readonly IValidator<LoginUserDto> _loginValidatorMock;
    private readonly AuthService _authService;
    
    public AuthServiceTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _passwordHasherMock = Substitute.For<IPasswordHasher>();
        _jwtProviderMock = Substitute.For<IJwtProvider>();
        _registerValidatorMock = Substitute.For<IValidator<RegisterUserDto>>();
        _loginValidatorMock = Substitute.For<IValidator<LoginUserDto>>();
        
        _authService = new AuthService(
            _userRepositoryMock,
            _passwordHasherMock,
            _jwtProviderMock,
            _registerValidatorMock,
            _loginValidatorMock);
    }
    
    [Fact]
    public async Task RegisterAsync_ShouldReturnValidationError_WhenDtoIsInvalid()
    {
        // Arrange
        var dto = new RegisterUserDto("", "", "", null, "", UserRole.Junior);
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("FirstName", "First name is required"),
            new ValidationFailure("LastName", "Last name is required"),
            new ValidationFailure("Email", "Email is required"),
            new ValidationFailure("Password", "Password is required")
        });
        
        _registerValidatorMock.ValidateAsync(dto).Returns(validationResult);

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(
            Error.ValidationFailed("First name is required, Last name is required, Email is required, Password is required"));
    }
    
    [Fact]
    public async Task RegisterAsync_ShouldReturnDuplicateError_WhenEmailExists()
    {
        // Arrange
        var validDto = new RegisterUserDto("John", "Doe", "test@example.com", "1234567890", "password", UserRole.Junior);
        _registerValidatorMock.ValidateAsync(validDto).Returns(new ValidationResult());
        _userRepositoryMock.ExistsByEmailAsync(validDto.Email).Returns(true);

        // Act
        var result = await _authService.RegisterAsync(validDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.Duplicate("Email", validDto.Email));
    }
    
    [Fact]
    public async Task RegisterAsync_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var validDto = new RegisterUserDto("John", "Doe", "test@example.com", "1234567890", "password", UserRole.Junior);
        var hashedPassword = "hashed_password";
        var expectedUser = new User
        {
            FirstName = validDto.FirstName,
            LastName = validDto.LastName,
            Email = validDto.Email,
            PhoneNumber = validDto.PhoneNumber,
            PasswordHash = hashedPassword,
            Role = validDto.Role
        };

        _registerValidatorMock.ValidateAsync(validDto).Returns(new ValidationResult());
        _userRepositoryMock.ExistsByEmailAsync(validDto.Email).Returns(false);
        _passwordHasherMock.Generate(validDto.Password).Returns(hashedPassword);
        _userRepositoryMock.AddAsync(Arg.Any<User>()).Returns(Task.CompletedTask)
            .AndDoes(c => expectedUser.Id = c.Arg<User>().Id);

        // Act
        var result = await _authService.RegisterAsync(validDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(expectedUser, 
            options => options.Excluding(u => u.Id));
        await _userRepositoryMock.Received(1).AddAsync(Arg.Any<User>());
    }
    
    [Fact]
    public async Task LoginAsync_ShouldReturnValidationError_WhenDtoIsInvalid()
    {
        // Arrange
        var dto = new LoginUserDto("", "");
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Email", "Email is required"),
            new ValidationFailure("Password", "Password is required")
        });
    
        _loginValidatorMock.ValidateAsync(dto).Returns(validationResult);

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(
            Error.ValidationFailed("Email is required, Password is required"));
    }
    
    [Fact]
    public async Task LoginAsync_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        // Arrange
        var dto = new LoginUserDto("test@example.com", "password");
        _loginValidatorMock.ValidateAsync(dto).Returns(new ValidationResult());
        _userRepositoryMock.GetByEmailAsync(dto.Email).Returns((User?)null);

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.Unauthorized("Invalid email or password"));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnUnauthorized_WhenPasswordIsInvalid()
    {
        // Arrange
        var dto = new LoginUserDto("test@example.com", "password");
        var user = new User { Email = dto.Email, PasswordHash = "hashed_password" };
    
        _loginValidatorMock.ValidateAsync(dto).Returns(new ValidationResult());
        _userRepositoryMock.GetByEmailAsync(dto.Email).Returns(user);
        _passwordHasherMock.Verify(dto.Password, user.PasswordHash).Returns(false);

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.Unauthorized("Invalid email or password"));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var dto = new LoginUserDto("test@example.com", "password");
        var user = new User { Email = dto.Email, PasswordHash = "hashed_password" };
        var expectedToken = "generated_token";
    
        _loginValidatorMock.ValidateAsync(dto).Returns(new ValidationResult());
        _userRepositoryMock.GetByEmailAsync(dto.Email).Returns(user);
        _passwordHasherMock.Verify(dto.Password, user.PasswordHash).Returns(true);
        _jwtProviderMock.GenerateToken(user).Returns(expectedToken);

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(expectedToken);
    }
}