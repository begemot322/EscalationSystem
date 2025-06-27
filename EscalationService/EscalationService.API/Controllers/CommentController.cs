using EscalationService.Appliacation.DTOs;
using EscalationService.Appliacation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EscalationService.API.Controllers;


[ApiController]
[Route("api/escalations/{escalationId}/[controller]")]
public class CommentController : BaseController
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetByEscalationId(int escalationId)
    {
        var result = await _commentService.GetByEscalationIdAsync(escalationId);
        if (result.IsSuccess)
            return Ok(result.Data);

        return Problem(result.Error!);
    }
    
    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetByUserId(int escalationId, int userId)
    {
        var result = await _commentService.GetByUserIdAsync(userId);
        if (result.IsSuccess)
            return Ok(result.Data);

        return Problem(result.Error!);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(int escalationId, [FromQuery] int userId, [FromBody] CommentDto dto)
    {
        var result = await _commentService.CreateAsync(dto, escalationId, userId);
        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetByEscalationId), new { escalationId }, result.Data);

        return Problem(result.Error!);
    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int escalationId, int id, [FromQuery] int userId, [FromBody] CommentDto dto)
    {
        var result = await _commentService.UpdateAsync(id, dto, userId);
        if (result.IsSuccess)
            return Ok(result.Data);

        return Problem(result.Error!);
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int escalationId, int id, [FromQuery] int userId)
    {
        var result = await _commentService.DeleteAsync(id, userId);
        if (result.IsSuccess)
            return NoContent();

        return Problem(result.Error!);
    }
}