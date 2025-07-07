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
    IUnitOfWork unitOfWork,
    IValidator<EscalationDto> validator,
    IUserServiceClient userServiceClient,
    IUserContext userContext,
    IMessageBusPublisher messageBusPublisher,
    IMapper mapper) : IEscalationService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IValidator<EscalationDto> _validator = validator;
    private readonly IMapper _mapper = mapper;
    private readonly IUserServiceClient _httpClientFactory = userServiceClient;
    private readonly IUserContext _userContext = userContext;
    private readonly IMessageBusPublisher _messageBusPublisher = messageBusPublisher;


    public async Task<Result<PagedResult<Escalation>>> GetAllEscalationsAsync(
        EscalationFilter? filter = null,
        SortParams? sortParams = null,
        PageParams? pageParams = null)
    {
        var result = await _unitOfWork.Escalations.GetAllAsync(filter, sortParams, pageParams);
        return Result<PagedResult<Escalation>>.Success(result);
    }

    public async Task<Result<Escalation>> GetEscalationByIdAsync(int id)
    {
        if (id <= 0)
        {
            return Result<Escalation>.Failure(
                Error.ValidationFailed("ID must be a positive number"));
        }
                
        var escalation = await _unitOfWork.Escalations.GetByIdAsync(id);

        if (escalation == null)
        {
            return Result<Escalation>.Failure(
                Error.NotFound<Escalation>(id));
        }
        return Result<Escalation>.Success(escalation); 
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
        
        await _unitOfWork.Escalations.AddAsync(escalation);
        
        var escalationUsers = dto.ResponsibleUserIds.Select(userId => new EscalationUser
        {
            EscalationId = escalation.Id,
            UserId = userId
        }).ToList();
        
        await _unitOfWork.EscalationUsers.AddRangeAsync(escalationUsers);
        await _unitOfWork.SaveChangesAsync();
        
        await _messageBusPublisher.PublishUserIds(dto.ResponsibleUserIds);
        
        return Result<Escalation>.Success(escalation);
    }

    public async Task<Result<Escalation>> UpdateEscalationAsync(int id, EscalationDto dto)
    {
        var existing = await _unitOfWork.Escalations.GetByIdAsync(id);
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
        
        await _unitOfWork.EscalationUsers.DeleteByEscalationIdAsync(id);

        var escalationUsers = dto.ResponsibleUserIds.Select(userId => new EscalationUser
        {
            EscalationId = id,
            UserId = userId
        }).ToList();
        
        await _unitOfWork.EscalationUsers.AddRangeAsync(escalationUsers);
        await _unitOfWork.Escalations.UpdateAsync(existing);
        await _unitOfWork.SaveChangesAsync();
        
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
        
        if (!await _unitOfWork.Escalations.ExistsAsync(id))
            return Result.Failure(Error.NotFound<Escalation>(id));

        var escalation = await _unitOfWork.Escalations.GetByIdAsync(id);
        await _unitOfWork.Escalations.DeleteAsync(escalation);
        await _unitOfWork.SaveChangesAsync();
    
        return Result.Success();
    }
    
    public async Task<Result<List<EscalationDtoMessage>>> GetFilteredEscalationsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        EscalationStatus? status = null)
    {
        var escalations = await _unitOfWork.Escalations.GetFilteredEscalationsAsync(fromDate, toDate, status);
        var escalationDtoMessages = _mapper.Map<List<EscalationDtoMessage>>(escalations);
        
        return Result<List<EscalationDtoMessage>>.Success(escalationDtoMessages);
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