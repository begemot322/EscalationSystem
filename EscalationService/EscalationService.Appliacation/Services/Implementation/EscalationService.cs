using AutoMapper;
using EscalationService.Appliacation.Common.Interfaces;
using EscalationService.Appliacation.Common.Interfaces.Repositories;
using EscalationService.Appliacation.Filters;
using EscalationService.Appliacation.Services.Interfaces;
using EscalationService.Domain.Entities;
using FluentValidation;
using Models;
using Models.DTOs;
using Models.QueryParams;
using Models.Result;
using EscalationDto = EscalationService.Appliacation.DTOs.EscalationDto;

namespace EscalationService.Appliacation.Services.Implementation;


public class EscalationService(
    IEscalationRepository repository,
    IValidator<EscalationDto> validator,
    IUserServiceClient userServiceClient,
    IEscalationUserRepository escalationUserRepository,
    IUserContext userContext,
    IMessageBusPublisher messageBusPublisher,
    IMapper mapper) : IEscalationService
{
    private readonly IEscalationRepository _repository = repository;
    private readonly IValidator<EscalationDto> _validator = validator;
    private readonly IMapper _mapper = mapper;
    private readonly IUserServiceClient _httpClientFactory = userServiceClient;
    private readonly IEscalationUserRepository _escalationUserRepository = escalationUserRepository;
    private readonly IUserContext _userContext = userContext;
    private readonly IMessageBusPublisher _messageBusPublisher = messageBusPublisher;


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
    
    public async Task<Result<Escalation>> CreateEscalationAsync(EscalationDto dto)
    {
        if (!CanCreateEscalation())
        {
            return Result<Escalation>.Failure(
                Error.Forbidden("Only Middle and Senior levels can create escalations"));
        }
        
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return Result<Escalation>.Failure(
                Error.ValidationFailed(string.Join(", ", 
                    validationResult.Errors.Select(e => e.ErrorMessage))));

        var usersExist = await _httpClientFactory.CheckUsersExistAsync(dto.ResponsibleUserIds);
        if (!usersExist)
            return Result<Escalation>.Failure(Error.NotFound("User", "one or more responsible users not found"));


        var escalation = _mapper.Map<Escalation>(dto, opt => opt.AfterMap((src, dest) => 
        {
            dest.CreatedAt = DateTime.UtcNow;
            dest.UpdatedAt = DateTime.UtcNow;
            dest.AuthorId = _userContext.GetUserId();
        }));
        
        await _repository.AddAsync(escalation);
        
        foreach (var userId in dto.ResponsibleUserIds)
        {
            await _escalationUserRepository.AddAsync(new EscalationUser
            {
                EscalationId = escalation.Id,
                UserId = userId
            });
        }
        
        foreach (var id in dto.ResponsibleUserIds)
        {
            Console.WriteLine($"ID: {id}, Тип: {id.GetType()?.Name}");
        }
        
        await _messageBusPublisher.PublishUserIds(dto.ResponsibleUserIds);
        
        return Result<Escalation>.Success(escalation);
    }

    public async Task<Result<Escalation>> UpdateEscalationAsync(int id, EscalationDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return Result<Escalation>.Failure(Error.NotFound<Escalation>(id));
        
        if (!CanUpdateEscalation(existing))
            return Result<Escalation>.Failure(Error.Forbidden("Only creator or Senior can update"));

        if (id <= 0)
            return Result<Escalation>.Failure(Error.ValidationFailed("ID must be a positive number"));
        
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return Result<Escalation>.Failure(
                Error.ValidationFailed(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
        
        var usersExist = await _httpClientFactory.CheckUsersExistAsync(dto.ResponsibleUserIds);
        if (!usersExist)
            return Result<Escalation>.Failure(Error.NotFound("User", "one or more responsible users not found"));
        
        _mapper.Map(dto, existing, opt => opt.AfterMap((src, dest) => 
        {
            dest.UpdatedAt = DateTime.UtcNow;
        }));
        
        await _repository.UpdateAsync(existing);
        
        await _escalationUserRepository.DeleteByEscalationIdAsync(id);
        
        foreach (var userId in dto.ResponsibleUserIds)
        {
            await _escalationUserRepository.AddAsync(new EscalationUser
            {
                EscalationId = id,
                UserId = userId
            });
        }
        
        return Result<Escalation>.Success(existing);
    }
    
    public async Task<Result> DeleteEscalationAsync(int id)
    {
        if (_userContext.GetUserRole() != "Senior")
        {
            return Result.Failure(Error.Forbidden("Only Senior can delete escalations"));
        }
        
        if (id <= 0)
            return Result.Failure(Error.ValidationFailed("ID must be a positive number"));
        
        if (!await _repository.ExistsAsync(id))
            return Result.Failure(Error.NotFound<Escalation>(id));

        var escalation = await _repository.GetByIdAsync(id);
        await _repository.DeleteAsync(escalation!);
    
        return Result.Success();
    }
    
    public async Task<Result<List<Models.DTOs.EscalationDto>>> GetFilteredEscalationsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        EscalationStatus? status = null)
    {
        var escalations = await _repository.GetFilteredEscalationsAsync(fromDate, toDate, status);
        var dtos = _mapper.Map<List<Models.DTOs.EscalationDto>>(escalations);
        
        return Result<List<Models.DTOs.EscalationDto>>.Success(dtos);
    }
    
    private bool CanCreateEscalation()
    {
        var userRole = _userContext.GetUserRole();
    
        if (userRole == "Middle" || userRole == "Senior")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    private bool CanUpdateEscalation(Escalation escalation)
    {
        var userRole = _userContext.GetUserRole();
        var userId = _userContext.GetUserId();

        if (userRole == "Senior")
            return true;

        if (userId == escalation.AuthorId)
            return true;

        return false;
    }
}