using EscalationService.Appliacation.DTOs.Criteria;
using EscalationService.Appliacation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EscalationService.API.Controllers;

[ApiController]
[Route("api/escalations/{escalationId}/[controller]")]
[Authorize]
public class CriteriaController : BaseController
{
    private readonly ICriteriaService _criteriaService;

    public CriteriaController(ICriteriaService criteriaService)
    {
        _criteriaService = criteriaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetByEscalationId(int escalationId)
    {
        var result = await _criteriaService.GetByEscalationIdAsync(escalationId);
        if (result.IsSuccess)
            return Ok(result.Data);

        return Problem(result.Error!);
    }
    
    [HttpPost]
    [Authorize(Roles = "Middle,Senior")]
    public async Task<IActionResult> Create(int escalationId, [FromBody] CreateCriteriaDto dto)
    {
        var result = await _criteriaService.CreateAsync(escalationId, dto);
        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetByEscalationId), new { escalationId }, result.Data);

        return Problem(result.Error!);
    }
    
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Middle,Senior")]
    public async Task<IActionResult> Update(int id,[FromBody] UpdateCriteriaDto dto)
    {
        var result = await _criteriaService.UpdateAsync(id, dto);
        if (result.IsSuccess)
            return Ok(result.Data);

        return Problem(result.Error!);
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Middle,Senior")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _criteriaService.DeleteAsync(id);
        if (result.IsSuccess)
            return NoContent();

        return Problem(result.Error!);
    }
}