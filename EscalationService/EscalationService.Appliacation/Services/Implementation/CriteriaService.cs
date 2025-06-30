using AutoMapper;
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
    IMapper mapper) : ICriteriaService
{
    private readonly ICriteriaRepository _repository = repository;
    private readonly IEscalationRepository _escalationRepository = escalationRepository;
    private readonly IValidator<CreateCriteriaDto> _createValidator = createValidator;
    private readonly IValidator<UpdateCriteriaDto> _updateValidator = updateValidator;
    private readonly IMapper _mapper = mapper;
    
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
    
    // Пока буду айди автора принимать в запросе, когда сделаю аунтефикацию то буду из jwt брать
    public async Task<Result<Criteria>> CreateAsync(int escalationId, int authorId, CreateCriteriaDto dto)
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
        
        if (escalation.AuthorId != authorId)
            return Result<Criteria>.Failure(Error.Forbidden("Only the author can add criteria"));
        
        var order = await _repository.CountByEscalationIdAsync(escalationId) + 1;  // +1 так как не хочу чтобы с 0 начиналось
        
        var criteria = _mapper.Map<Criteria>(dto);
        criteria.Order = order;
        criteria.EscalationId = escalationId;
        criteria.IsCompleted = false;
        
        await _repository.AddAsync(criteria);
        return Result<Criteria>.Success(criteria);
    }

    // тут пока тоже самое
    public async Task<Result<Criteria>> UpdateAsync(int id, UpdateCriteriaDto dto, int authorId)
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
        
        if (escalation.AuthorId != authorId)
            return Result<Criteria>.Failure(Error.Forbidden("Only the author can update criteria"));

        _mapper.Map(dto, criteria);
        
        await _repository.UpdateAsync(criteria);
        return Result<Criteria>.Success(criteria);
    }

    // и тут пока тоже
    public async Task<Result> DeleteAsync(int id, int authorId)
    {
        if (id <= 0)
            return Result<Criteria>.Failure(Error.ValidationFailed("Criteria ID must be a positive number"));
        
        var criteria = await _repository.GetByIdAsync(id);
        if (criteria is null)
            return Result.Failure(Error.NotFound<Criteria>(id));
        
        var escalation = await _escalationRepository.GetByIdAsync(criteria.EscalationId);
        if (escalation is null)
            return Result.Failure(Error.NotFound<Escalation>(criteria.EscalationId));
        
        if (escalation.AuthorId != authorId)
            return Result.Failure(Error.Forbidden("Only the author can delete criteria"));
        
        await _repository.DeleteAsync(criteria);
        return Result.Success();
    }
    
}