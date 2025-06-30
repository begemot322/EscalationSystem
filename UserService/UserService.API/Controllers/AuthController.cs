using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Application.DTOs;
using UserService.Application.Services.Interfaces;
using UserService.Infrastructure.Data;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : BaseController
{
    private readonly ApplicationDbContext _db;
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService, ApplicationDbContext db)
    {
        _authService = authService;
        _db = db;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        if (result.IsSuccess)
            return Ok(result.Data);

        return Problem(result.Error);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (result.IsSuccess)
        {
            Response.Cookies.Append("SecurityCookies", result.Data);
            return Ok(new { Message = "Authenticated successfully" });
        }
        return Problem(result.Error);
    }
    
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Response.Cookies.Delete("SecurityCookies");
        
        return Ok(new { Message = "Logged out successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> GetUser()
    {
        var users = await _db.Users.ToListAsync();
        return Ok(users);
    }
}