using EscalationService.Appliacation.Models.DTOs;
using EscalationService.Appliacation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EscalationService.API.Controllers;


[ApiController]
[Route("api/escalation/{escalationId}/[controller]")]
[Authorize] 
public class CommentController : BaseController
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }
    
    [HttpGet]
    [Authorize(Roles = "Junior,Middle,Senior")]
    public async Task<IActionResult> GetByEscalationId(int escalationId)
    {
        var result = await _commentService.GetByEscalationIdAsync(escalationId);
        if (result.IsSuccess)
            return Ok(result.Data);

        return Problem(result.Error!);
    }
    
    [HttpGet("user/{userId:int}")]
    [Authorize(Roles = "Junior,Middle,Senior")]
    public async Task<IActionResult> GetByUserId(int escalationId, int userId)
    {
        var result = await _commentService.GetByUserIdAsync(userId);
        if (result.IsSuccess)
            return Ok(result.Data);

        return Problem(result.Error!);
    }
    
    [HttpPost]
    [Authorize(Roles = "Junior,Middle,Senior")] 
    public async Task<IActionResult> Create(int escalationId, [FromBody] CommentDto dto)
    {
        var result = await _commentService.CreateAsync(dto, escalationId);
        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetByEscalationId), new { escalationId }, result.Data);

        return Problem(result.Error!);
    }
    
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Junior,Middle,Senior")]
    public async Task<IActionResult> Update(int id, [FromBody] CommentDto dto)
    {
        var result = await _commentService.UpdateAsync(id, dto);
        if (result.IsSuccess)
            return Ok(result.Data);

        return Problem(result.Error!);
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Junior,Middle,Senior")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _commentService.DeleteAsync(id);
        if (result.IsSuccess)
            return NoContent();

        return Problem(result.Error!);
    }
}