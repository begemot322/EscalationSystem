namespace UserService.Application.Common.Identity;

public interface IPasswordHasher
{
    string Generate(string password);
    bool Verify(string password, string HashPassword);
}