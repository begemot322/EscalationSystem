using EscalationService.Appliacation.Common.Interfaces.Repositories;
using EscalationService.Appliacation.DTOs;
using EscalationService.Appliacation.Filters;
using EscalationService.Appliacation.Services.Interfaces;
using EscalationService.Domain.Entities;
using FluentValidation;
using Models.QueryParams;
using Models.Result;

namespace EscalationService.Appliacation.Services.Implementation;


public class EscalationService(IEscalationRepository repository, IValidator<EscalationDto> validator) 
    : IEscalationService
{
    private readonly IEscalationRepository _repository = repository;
    private readonly IValidator<EscalationDto> _validator = validator;

    public async Task<Result<PagedResult<Escalation>>> GetAllEscalationsAsync(
        EscalationFilter? filter = null,
        SortParams? sortParams = null,
        PageParams? pageParams = null)
    {
        try
        {
            var result = await _repository.GetAllAsync(filter, sortParams, pageParams);
            return Result<PagedResult<Escalation>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<Escalation>>.Failure(
                Error.InternalServerError($"Failed to get escalations: {ex.Message}"));
        }
    }

    public async Task<Result<Escalation>> GetEscalationByIdAsync(int id)
    {
        if (id <= 0)
        {
            return Result<Escalation>.Failure(
                Error.ValidationFailed("ID must be a positive number"));
        }
                
        var escalation = await _repository.GetByIdAsync(id);

        if (escalation == null)
        {
            return Result<Escalation>.Failure(
                Error.NotFound<Escalation>(id));
        }
        else
        {
            return Result<Escalation>.Success(escalation); 
        }
    }
    
    public async Task<Result<Escalation>> CreateEscalationAsync(EscalationDto  dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return Result<Escalation>.Failure(
                Error.ValidationFailed(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
        }
        
        try
        {
            var escalation = new Escalation
            {
                Name = dto.Name,
                Description = dto.Description,
                Status = dto.Status,
                AuthorId = dto.AuthorId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(escalation);
            return Result<Escalation>.Success(escalation);
        }
        catch (Exception ex)
        {
            return Result<Escalation>.Failure(
                Error.InternalServerError($"Failed to create escalation: {ex.Message}"));
        }
    }

    public async Task<Result<Escalation>> UpdateEscalationAsync(int id, EscalationDto dto)
    {
        if (id <= 0)
            return Result<Escalation>.Failure(
                Error.ValidationFailed("ID must be a positive number"));
        
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return Result<Escalation>.Failure(
                Error.ValidationFailed(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
        }
        
        try
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing is null)
                return Result<Escalation>.Failure(Error.NotFound<Escalation>(id));

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Status = dto.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existing);
            return Result<Escalation>.Success(existing);
        }
        catch (Exception ex)
        {
            return Result<Escalation>.Failure(
                Error.InternalServerError($"Failed to update escalation: {ex.Message}"));
        }
    }
    
    public async Task<Result> DeleteEscalationAsync(int id)
    {
        if (id <= 0)
            return Result.Failure(
                Error.ValidationFailed("ID must be a positive number"));
        
        try
        {
            if (!await _repository.ExistsAsync(id))
                return Result.Failure(Error.NotFound<Escalation>(id));

            var escalation = await _repository.GetByIdAsync(id);
            await _repository.DeleteAsync(escalation!);
        
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(
                Error.InternalServerError($"Failed to delete escalation: {ex.Message}"));
        }
    }
}