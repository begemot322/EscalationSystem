using Models;
using UserService.Domain;

namespace UserService.Application.DTOs;

public record RegisterUserDto(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string Password,
    UserRole Role);