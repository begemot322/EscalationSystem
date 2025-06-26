using EscalationService.Appliacation.DTOs.Criteria;
using EscalationService.Appliacation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EscalationService.API.Controllers;

[ApiController]
[Route("api/escalations/{escalationId}/[controller]")]
public class CriteriaController : BaseController
{
    private readonly ICriteriaService _criteriaService;

    public CriteriaController(ICriteriaService criteriaService)
    {
        _criteriaService = criteriaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int escalationId)
    {
        var result = await _criteriaService.GetByEscalationIdAsync(escalationId);
        if (result.IsSuccess)
            return Ok(result.Data);

        return Problem(result.Error!);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(int escalationId, [FromQuery] int authorId, [FromBody] CreateCriteriaDto dto)
    {
        var result = await _criteriaService.CreateAsync(escalationId, authorId, dto);
        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetAll), new { escalationId }, result.Data);

        return Problem(result.Error!);
    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int escalationId, int id, [FromQuery] int authorId, [FromBody] UpdateCriteriaDto dto)
    {
        var result = await _criteriaService.UpdateAsync(id, dto, authorId);
        if (result.IsSuccess)
            return Ok(result.Data);

        return Problem(result.Error!);
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int escalationId, int id, [FromQuery] int authorId)
    {
        var result = await _criteriaService.DeleteAsync(id, authorId);
        if (result.IsSuccess)
            return NoContent();

        return Problem(result.Error!);
    }
}