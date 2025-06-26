using EscalationService.Appliacation.DTOs;
using EscalationService.Appliacation.Filters;
using EscalationService.Appliacation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.QueryParams;

namespace EscalationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EscalationController : ControllerBase
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
        
        return result.IsSuccess
            ? Ok(result.Data)
            : BadRequest(result.Error?.Message);
    }
    
    [HttpGet("{id}")]
    [ActionName("GetById")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _escalationService.GetEscalationByIdAsync(id);
        return result.IsSuccess
            ? Ok(result.Data)
            : NotFound(result.Error?.Message);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddEscalation([FromBody] EscalationDto dto)
    {
        var result = await _escalationService.CreateEscalationAsync(dto);
        
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result.Data)
            : BadRequest(result.Error?.Message);
    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EscalationDto dto)
    {
        var result = await _escalationService.UpdateEscalationAsync(id, dto);
        return result.IsSuccess
            ? Ok(result.Data)
            : BadRequest(result.Error?.Message);
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _escalationService.DeleteEscalationAsync(id);
        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.Error?.Message);
    }
    
    
}