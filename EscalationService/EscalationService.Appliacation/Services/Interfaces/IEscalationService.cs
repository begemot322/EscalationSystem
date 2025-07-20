using EscalationService.Appliacation.DTOs;
using EscalationService.Appliacation.Filters;
using EscalationService.Domain.Entities;
using Models;
using Models.DTOs;
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
    Task<Result<Escalation>> UpdateEscalationAsync(int id, UpdateEscalationDto dto);
    Task<Result> DeleteEscalationAsync(int id);
    Task<Result<List<EscalationReminderDto>>> GetOverdueEscalationsAsync();
    public Task<Result<List<EscalationDtoMessage>>> GetFilteredEscalationsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        EscalationStatus? status = null);

    Task<Result<PagedResult<Escalation>>> GetCreatedEscalationsAsync(PageParams? pageParams = null);
    Task<Result<PagedResult<Escalation>>> GetUserEscalationsAsync(PageParams? pageParams = null);
}
