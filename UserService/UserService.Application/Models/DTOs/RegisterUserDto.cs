using Models;

namespace UserService.Application.Models.DTOs;

public record RegisterUserDto(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string Password,
    UserRole Role);