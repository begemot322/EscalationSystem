using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using UserService.Application.Common.Interfaces;

namespace UserService.Infrastructure.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public int GetUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
        return int.Parse(userIdClaim);
    }

    public string GetUserRole()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
    }

    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }
}