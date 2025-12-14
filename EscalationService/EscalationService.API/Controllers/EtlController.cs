using EscalationService.Appliacation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EscalationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EtlController : ControllerBase
{
    private readonly IEtlService _etlService;

    public EtlController(IEtlService etlService)
    {
        _etlService = etlService;
    }

    [HttpPost("import-csv")]
    public async Task<IActionResult> ImportCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(uploadsFolder);
        var filePath = Path.Combine(uploadsFolder, file.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var result = await _etlService.ImportEscalationsFromCsvAsync(filePath);

        System.IO.File.Delete(filePath);

        return Ok(new
        {
            SuccessCount = result.Successful.Count,
            FailureCount = result.Failures.Count,
            Details = result.ToString()
        });
    }
}