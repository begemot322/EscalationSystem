using EscalationService.Appliacation.DTOs;
using EscalationService.Appliacation.Filters;
using EscalationService.Domain.Entities;
using Models.QueryParams;
using Models.Result;

namespace EscalationService.Appliacation.Services.Interfaces;

public interface IEscalationService
{
    Task<Result<PagedResult<Escalation>>> GetAllEscalationsAsync(
        EscalationFilter? filter = null,
        SortParams? sortParams = null,
        PageParams? pageParams = null);

    Task<Result<Escalation>> GetEscalationByIdAsync(int id);
    Task<Result<Escalation>> CreateEscalationAsync(EscalationDto  dto);
    Task<Result<Escalation>> UpdateEscalationAsync(int id, EscalationDto dto);
    Task<Result> DeleteEscalationAsync(int id);
}
