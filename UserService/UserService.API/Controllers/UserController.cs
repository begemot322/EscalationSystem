using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Models.QueryParams;
using UserService.Application.Models.Filters;
using UserService.Application.Services.Interfaces;
using UserService.Domain;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet]
    [Authorize(Roles = "Middle,Senior")] 
    public async Task<ActionResult<PagedResult<User>>> GetAll(
        [FromQuery] UserFilter? filter = null,
        [FromQuery] PageParams? pageParams = null)
    {
        var result = await _userService.GetAllUsersAsync(filter, pageParams);

        if (!result.IsSuccess)
            return NotFound(result.Error); 

        return Ok(result.Data);
    }
    
    [HttpGet("{id:int}")]
    [Authorize] 
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Data);
    }
}