using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using ReportingService.API.Services;

namespace ReportingService.API.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize] 
public class ReportController : ControllerBase
{
    private readonly ReportService _reportService;
    
    public ReportController(ReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpPost("create")]
    [Authorize(Roles = "Middle,Senior")]
    public async Task<IActionResult> GenerateReport([FromBody] ReportRequest request)
    {
        try
        {
            var pdfBytes = await _reportService.GenerateReportAsync(
                request.FromDate, 
                request.ToDate, 
                request.Status);

            string fileName = $"Отчет_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid("У вас нет прав для генерации отчета.");
        }
    }
}