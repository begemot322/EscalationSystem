using EscalationService.Appliacation.DTOs.Criteria;
using EscalationService.Domain.Entities;
using Models.Result;

namespace EscalationService.Appliacation.Services.Interfaces;

public interface ICriteriaService
{
    Task<Result<IEnumerable<Criteria>>> GetByEscalationIdAsync(int escalationId);
    Task<Result<Criteria>> CreateAsync(int escalationId, int authorId, CreateCriteriaDto dto);
    Task<Result<Criteria>> UpdateAsync(int id, UpdateCriteriaDto dto, int authorId);
    Task<Result> DeleteAsync(int id, int authorId);
}
