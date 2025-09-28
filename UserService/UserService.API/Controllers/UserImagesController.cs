using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Models.DTOs;
using UserService.Application.Services.Interfaces;

namespace UserService.API.Controllers;

[ApiController]
[Authorize]
public class UserImagesController : Controller
{
    private readonly IUserImageService _userImageService;

    public UserImagesController(IUserImageService userImageService)
    {
        _userImageService = userImageService;
    }
    
    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required");

        await using var stream = file.OpenReadStream();
        var fileUploadDto = new FileUploadDto
        {
            FileName = file.FileName,
            InputStream = stream,
            ContentType = file.ContentType
        };

        var result = await _userImageService.UploadUserAvatarAsync(fileUploadDto);
        
        return result.IsSuccess 
            ? Ok(new { FilePath = result.Data })
            : BadRequest(result.Error);
    }
    
    [HttpDelete("avatar")]
    public async Task<IActionResult> DeleteAvatar()
    {
        var result = await _userImageService.DeleteUserAvatarAsync();
        
        return result.IsSuccess 
            ? NoContent() 
            : BadRequest(result.Error);
    }
}