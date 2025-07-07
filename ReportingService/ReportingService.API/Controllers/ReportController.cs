using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using ReportingService.API.Services;

namespace ReportingService.API.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportController : ControllerBase
{
    private readonly ReportService _reportService;
    
    public ReportController(ReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> GenerateReport([FromBody] ReportRequest request)
    {
        var pdfBytes = await _reportService.GenerateReportAsync(
            request.FromDate, 
            request.ToDate, 
            request.Status);
        
        string fileName = $"Отчет_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        
        return File(pdfBytes, "application/pdf", fileName);
    }
}