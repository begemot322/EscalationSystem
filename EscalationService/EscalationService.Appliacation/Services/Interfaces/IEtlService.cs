using EscalationService.Appliacation.Models.DTOs;

namespace EscalationService.Appliacation.Services.Interfaces;

public interface IEtlService
{
    Task<ImportResult> ImportEscalationsFromCsvAsync(string filePath);
}