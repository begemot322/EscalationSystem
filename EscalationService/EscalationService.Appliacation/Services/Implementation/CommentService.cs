using AutoMapper;
using EscalationService.Appliacation.Common.Interfaces;
using EscalationService.Appliacation.Models.DTOs;
using EscalationService.Appliacation.Services.Interfaces;
using EscalationService.Domain.Entities;
using FluentValidation;
using Models.Result;

namespace EscalationService.Appliacation.Services.Implementation;

public class CommentService(
    IUnitOfWork unitOfWork,
    IValidator<CommentDto> validator,
    IUserContext userContext,
    IMapper mapper) : ICommentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IValidator<CommentDto> _validator = validator;
    private readonly IMapper _mapper = mapper;
    private readonly IUserContext _userContext = userContext;

    public async Task<Result<IEnumerable<Comment>>> GetByEscalationIdAsync(int escalationId)
    {
        if (escalationId <= 0)
            return Result<IEnumerable<Comment>>.Failure(Error.ValidationFailed("Escalation ID must be a positive number"));
        
        var exists = await _unitOfWork.Escalations.ExistsAsync(escalationId);
        if (!exists)
            return Result<IEnumerable<Comment>>.Failure(Error.NotFound<Escalation>(escalationId));
        
        var comments = await _unitOfWork.Comments.GetByEscalationIdAsync(escalationId);
        return Result<IEnumerable<Comment>>.Success(comments);
    }

    
    public async Task<Result<IEnumerable<Comment>>> GetByUserIdAsync(int userId)
    {
        if (userId <= 0)
            return Result<IEnumerable<Comment>>.Failure(Error.ValidationFailed("Escalation ID must be a positive number"));

        var comments = await _unitOfWork.Comments.GetByUserIdAsync(userId);
        return Result<IEnumerable<Comment>>.Success(comments);
    }
    
    public async Task<Result<Comment>> CreateAsync(CommentDto dto, int escalationId)
    {
        var validation = await _validator.ValidateAsync(dto);
        
        if (!validation.IsValid)
            return Result<Comment>.Failure(
                Error.ValidationFailed(string.Join(", ", validation.Errors.Select(e => e.ErrorMessage))));
        
        var userId = _userContext.GetUserId(); 
        
        var escalation = await _unitOfWork.Escalations.GetByIdAsync(escalationId);
        if (escalation is null)
            return Result<Comment>.Failure(Error.NotFound<Escalation>(escalationId));

        var comment = _mapper.Map<Comment>(dto, opt => opt.AfterMap((src, dest) => 
        {
            dest.EscalationId = escalationId;
            dest.UserId = userId;
            dest.CreatedAt = DateTime.UtcNow;
        }));
        
        await _unitOfWork.Comments.AddAsync(comment);
        await _unitOfWork.SaveChangesAsync(); 
        
        return Result<Comment>.Success(comment);
    }

    public async Task<Result<Comment>> UpdateAsync(int commentId, CommentDto dto)
    {
        if (commentId <= 0)
            return Result<Comment>.Failure(Error.ValidationFailed("Comment ID must be a positive number"));
        
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Result<Comment>.Failure(
                Error.ValidationFailed(string.Join(", ", validation.Errors.Select(e => e.ErrorMessage))));
        
        var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
        if (comment is null)
            return Result<Comment>.Failure(Error.NotFound<Comment>(commentId));
        
        var userId = _userContext.GetUserId();
        
        var userRole = _userContext.GetUserRole(); 
        if (userRole != "Senior" && comment.UserId != userId)
            return Result<Comment>.Failure(Error.Forbidden("You can only edit your own comments"));
        
        comment.Text = dto.Text;
        await _unitOfWork.Comments.UpdateAsync(comment);
        await _unitOfWork.SaveChangesAsync(); 
        
        return Result<Comment>.Success(comment);
    }

    public async Task<Result> DeleteAsync(int commentId)
    {
        if (commentId <= 0)
            return Result.Failure(Error.ValidationFailed("Comment ID must be a positive number"));
        
        var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
        if (comment is null)
            return Result.Failure(Error.NotFound<Comment>(commentId));
        
        var userId = _userContext.GetUserId();
        
        var userRole = _userContext.GetUserRole(); 
        if (userRole != "Senior" && comment.UserId != userId)
            return Result.Failure(Error.Forbidden("You can only delete your own comments"));
        
        await _unitOfWork.Comments.DeleteAsync(comment);
        await _unitOfWork.SaveChangesAsync(); 

        return Result.Success();
    }
}