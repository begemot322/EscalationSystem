namespace UserService.Application.Common.Interfaces.Identity;

public interface IPasswordHasher
{
    string Generate(string password);
    bool Verify(string password, string HashPassword);
}