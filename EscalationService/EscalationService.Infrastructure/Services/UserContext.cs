using System.Security.Claims;
using EscalationService.Appliacation.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace EscalationService.Infrastructure.Services;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

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