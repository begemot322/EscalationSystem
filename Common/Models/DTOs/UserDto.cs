namespace Models.DTOs;

public record UserDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    UserRole Role 
);
