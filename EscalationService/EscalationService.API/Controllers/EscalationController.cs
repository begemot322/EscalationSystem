using EscalationService.Appliacation.DTOs;
using EscalationService.Appliacation.Filters;
using EscalationService.Appliacation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.QueryParams;

namespace EscalationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EscalationController : BaseController
{
    private readonly IEscalationService _escalationService;

    public EscalationController(IEscalationService escalationService)
    {
        _escalationService = escalationService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] EscalationFilter? filter = null,
        [FromQuery] SortParams? sortParams = null,
        [FromQuery] PageParams? pageParams = null)
    {
        var result = await _escalationService.GetAllEscalationsAsync(filter, sortParams, pageParams);
        if (result.IsSuccess)
            return Ok(result.Data);

        return Problem(result.Error!);
    }
    
    [HttpGet("{id}")]
    [ActionName("GetById")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _escalationService.GetEscalationByIdAsync(id);
        if (result.IsSuccess)
            return Ok(result.Data);

        return Problem(result.Error!);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddEscalation([FromBody] EscalationDto dto)
    {
        var result = await _escalationService.CreateEscalationAsync(dto);
        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result.Data);

        return Problem(result.Error!);
    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EscalationDto dto)
    {
        var result = await _escalationService.UpdateEscalationAsync(id, dto);
        if (result.IsSuccess)
            return Ok(result.Data);

        return Problem(result.Error!);
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _escalationService.DeleteEscalationAsync(id);
        if (result.IsSuccess)
            return NoContent();

        return Problem(result.Error!);
    }
}