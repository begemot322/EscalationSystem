using UserService.Domain;

namespace UserService.Application.Common.Identity;

public interface IJwtProvider
{
    string GenerateToken(User user);
}