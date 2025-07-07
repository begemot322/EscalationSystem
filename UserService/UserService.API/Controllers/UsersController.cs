using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTOs;
using UserService.Application.Services.Interfaces;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;
    
    [HttpPost("check-exists")]
    public async Task<IActionResult> CheckUsersExist([FromBody] List<int> userIds)
    {
        var result = await _userService.CheckUsersExist(userIds);
        
        return Ok(result);
    }
    
    [HttpPost("by-ids")]
    public async Task<ActionResult<List<UserDto>>> GetUsersBasicInfo(
        [FromBody] List<int> userIds)
    {
        var users = await _userService.GetUsersInfo(userIds);
        return Ok(users);
    }
}