using AutoMapper;
using EscalationService.Appliacation.Common.Interfaces.Repositories;
using EscalationService.Appliacation.DTOs;
using EscalationService.Appliacation.Filters;
using EscalationService.Appliacation.Services.Interfaces;
using EscalationService.Domain.Entities;
using FluentValidation;
using Models.QueryParams;
using Models.Result;

namespace EscalationService.Appliacation.Services.Implementation;


public class EscalationService(
    IEscalationRepository repository,
    IValidator<EscalationDto> validator,
    IMapper mapper) : IEscalationService
{
    private readonly IEscalationRepository _repository = repository;
    private readonly IValidator<EscalationDto> _validator = validator;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<PagedResult<Escalation>>> GetAllEscalationsAsync(
        EscalationFilter? filter = null,
        SortParams? sortParams = null,
        PageParams? pageParams = null)
    {
        var result = await _repository.GetAllAsync(filter, sortParams, pageParams);
        return Result<PagedResult<Escalation>>.Success(result);
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
            return Result<Escalation>.Failure(
                Error.ValidationFailed(string.Join(", ", 
                    validationResult.Errors.Select(e => e.ErrorMessage))));

        var escalation = _mapper.Map<Escalation>(dto);
        escalation.CreatedAt = DateTime.UtcNow;
        escalation.UpdatedAt = DateTime.UtcNow;
        
        await _repository.AddAsync(escalation);
        return Result<Escalation>.Success(escalation);
    }

    public async Task<Result<Escalation>> UpdateEscalationAsync(int id, EscalationDto dto)
    {
        if (id <= 0)
            return Result<Escalation>.Failure(Error.ValidationFailed("ID must be a positive number"));
        
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return Result<Escalation>.Failure(
                Error.ValidationFailed(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
        
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            return Result<Escalation>.Failure(Error.NotFound<Escalation>(id));

        _mapper.Map(dto, existing);
        existing.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(existing);
        return Result<Escalation>.Success(existing);
    }
    
    public async Task<Result> DeleteEscalationAsync(int id)
    {
        if (id <= 0)
            return Result.Failure(Error.ValidationFailed("ID must be a positive number"));
        
        if (!await _repository.ExistsAsync(id))
            return Result.Failure(Error.NotFound<Escalation>(id));

        var escalation = await _repository.GetByIdAsync(id);
        await _repository.DeleteAsync(escalation!);
    
        return Result.Success();
    }
}