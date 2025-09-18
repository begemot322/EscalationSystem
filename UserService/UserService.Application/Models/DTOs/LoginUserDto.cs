namespace UserService.Application.Models.DTOs;

public record LoginUserDto(
    string Email, 
    string Password);