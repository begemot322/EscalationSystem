using AutoMapper;
using EscalationService.Appliacation.Common.Interfaces;
using EscalationService.Appliacation.Common.Interfaces.Repositories;
using EscalationService.Appliacation.DTOs.Criteria;
using EscalationService.Appliacation.Services.Interfaces;
using EscalationService.Domain.Entities;
using FluentValidation;
using Models.Result;

namespace EscalationService.Appliacation.Services.Implementation;

public class CriteriaService(
    ICriteriaRepository repository,
    IEscalationRepository escalationRepository,
    IValidator<CreateCriteriaDto> createValidator,
    IValidator<UpdateCriteriaDto> updateValidator,
    IUserContext userContext,
    IMapper mapper) : ICriteriaService
{
    private readonly ICriteriaRepository _repository = repository;
    private readonly IEscalationRepository _escalationRepository = escalationRepository;
    private readonly IValidator<CreateCriteriaDto> _createValidator = createValidator;
    private readonly IValidator<UpdateCriteriaDto> _updateValidator = updateValidator;
    private readonly IMapper _mapper = mapper;
    private readonly IUserContext _userContext = userContext;
    
    public async Task<Result<IEnumerable<Criteria>>> GetByEscalationIdAsync(int escalationId)
    {      
        if (escalationId <= 0)
        {
            return Result<IEnumerable<Criteria>>.Failure(
                Error.ValidationFailed("Escalation ID must be a positive number"));
        }
        
        var escalationExists = await _escalationRepository.ExistsAsync(escalationId);
        
        if (!escalationExists)
        {
            return Result<IEnumerable<Criteria>>.Failure(
                Error.NotFound<Escalation>(escalationId));
        }
        
        var list = await _repository.GetByEscalationIdAsync(escalationId);
        return Result<IEnumerable<Criteria>>.Success(list);
    }
    
    public async Task<Result<Criteria>> CreateAsync(int escalationId, CreateCriteriaDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return Result<Criteria>.Failure(
                Error.ValidationFailed(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
        }
        
        var escalation = await _escalationRepository.GetByIdAsync(escalationId);
        if (escalation is null)
            return Result<Criteria>.Failure(Error.NotFound<Escalation>(escalationId));
        
        if (!CanModifyEscalation(escalation))
            return Result<Criteria>.Failure(Error.Forbidden("No permissions to modify this escalation"));
        
        var order = await _repository.CountByEscalationIdAsync(escalationId) + 1; 
        
        var criteria = _mapper.Map<Criteria>(dto, opt => opt.AfterMap((src, dest) => 
        {
            dest.Order = order;
            dest.EscalationId = escalationId;
            dest.IsCompleted = false;
        }));
        
        await _repository.AddAsync(criteria);
        return Result<Criteria>.Success(criteria);
    }

    public async Task<Result<Criteria>> UpdateAsync(int id, UpdateCriteriaDto dto)
    {
        if (id <= 0)
            return Result<Criteria>.Failure(Error.ValidationFailed("Criteria ID must be a positive number"));
        
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return Result<Criteria>.Failure(
                Error.ValidationFailed(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
        }
        
        var criteria = await _repository.GetByIdAsync(id);
        if (criteria is null)
            return Result<Criteria>.Failure(Error.NotFound<Criteria>(id));
        
        var escalation = await _escalationRepository.GetByIdAsync(criteria.EscalationId);
        if (escalation is null)
            return Result<Criteria>.Failure(Error.NotFound<Escalation>(criteria.EscalationId));
        
        if (!CanModifyEscalation(escalation))
            return Result<Criteria>.Failure(Error.Forbidden("No permissions to modify this escalation"));

        _mapper.Map(dto, criteria);
        
        await _repository.UpdateAsync(criteria);
        return Result<Criteria>.Success(criteria);
    }

    public async Task<Result> DeleteAsync(int id)
    {
        if (id <= 0)
            return Result<Criteria>.Failure(Error.ValidationFailed("Criteria ID must be a positive number"));
        
        var criteria = await _repository.GetByIdAsync(id);
        if (criteria is null)
            return Result.Failure(Error.NotFound<Criteria>(id));
        
        var escalation = await _escalationRepository.GetByIdAsync(criteria.EscalationId);
        if (escalation is null)
            return Result.Failure(Error.NotFound<Escalation>(criteria.EscalationId));
        
        if (!CanModifyEscalation(escalation))
            return Result<Criteria>.Failure(Error.Forbidden("No permissions to modify this escalation"));
        
        await _repository.DeleteAsync(criteria);
        return Result.Success();
    }
    
    private bool CanModifyEscalation(Escalation escalation)
    {
        var userRole = _userContext.GetUserRole();
        var userId = _userContext.GetUserId();
        
        return userRole == "Senior" || escalation.AuthorId == userId;
    }
    
}