using AutoMapper;
using EscalationService.Appliacation.Common.Interfaces.Repositories;
using EscalationService.Appliacation.DTOs;
using EscalationService.Appliacation.Services.Interfaces;
using EscalationService.Domain.Entities;
using FluentValidation;
using Models.Result;

namespace EscalationService.Appliacation.Services.Implementation;

public class CommentService(
    ICommentRepository commentRepository,
    IEscalationRepository escalationRepository,
    IValidator<CommentDto> validator,
    IMapper mapper) : ICommentService
{
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly IEscalationRepository _escalationRepository = escalationRepository;
    private readonly IValidator<CommentDto> _validator = validator;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<IEnumerable<Comment>>> GetByEscalationIdAsync(int escalationId)
    {
        if (escalationId <= 0)
            return Result<IEnumerable<Comment>>.Failure(Error.ValidationFailed("Escalation ID must be a positive number"));
        
        var exists = await _escalationRepository.ExistsAsync(escalationId);
        if (!exists)
            return Result<IEnumerable<Comment>>.Failure(Error.NotFound<Escalation>(escalationId));
        
        var comments = await _commentRepository.GetByEscalationIdAsync(escalationId);
        return Result<IEnumerable<Comment>>.Success(comments);
    }

    
    public async Task<Result<IEnumerable<Comment>>> GetByUserIdAsync(int userId)
    {
        if (userId <= 0)
            return Result<IEnumerable<Comment>>.Failure(Error.ValidationFailed("Escalation ID must be a positive number"));

        var comments = await _commentRepository.GetByUserIdAsync(userId);
        return Result<IEnumerable<Comment>>.Success(comments);
    }
    
    public async Task<Result<Comment>> CreateAsync(CommentDto dto, int escalationId, int userId)
    {
        var validation = await _validator.ValidateAsync(dto);
        
        if (!validation.IsValid)
            return Result<Comment>.Failure(
                Error.ValidationFailed(string.Join(", ", validation.Errors.Select(e => e.ErrorMessage))));


        var escalation = await _escalationRepository.GetByIdAsync(escalationId);
        if (escalation is null)
            return Result<Comment>.Failure(Error.NotFound<Escalation>(escalationId));

        var comment = _mapper.Map<Comment>(dto);
        comment.EscalationId = escalationId;
        comment.UserId = userId;

        await _commentRepository.AddAsync(comment);
        return Result<Comment>.Success(comment);
    }

    public async Task<Result<Comment>> UpdateAsync(int commentId, CommentDto dto, int userId)
    {
        if (commentId <= 0)
            return Result<Comment>.Failure(Error.ValidationFailed("Comment ID must be a positive number"));
        
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Result<Comment>.Failure(
                Error.ValidationFailed(string.Join(", ", validation.Errors.Select(e => e.ErrorMessage))));
        
        var comment = await _commentRepository.GetByIdAsync(commentId);
        if (comment is null)
            return Result<Comment>.Failure(Error.NotFound<Comment>(commentId));
        
        var escalation = await _escalationRepository.GetByIdAsync(comment.EscalationId);
        if (escalation is null)
            return Result<Comment>.Failure(Error.NotFound<Escalation>(comment.EscalationId));
        
        if (comment.UserId != userId && escalation.AuthorId != userId)
            return Result<Comment>.Failure(Error.Forbidden("Only the author or escalation author can update the comment"));

        comment.Text = dto.Text;
        await _commentRepository.UpdateAsync(comment);

        return Result<Comment>.Success(comment);
    }

    public async Task<Result> DeleteAsync(int commentId, int userId)
    {
        if (commentId <= 0)
            return Result.Failure(Error.ValidationFailed("Comment ID must be a positive number"));
        
        var comment = await _commentRepository.GetByIdAsync(commentId);
        if (comment is null)
            return Result.Failure(Error.NotFound<Comment>(commentId));
        
        var escalation = await _escalationRepository.GetByIdAsync(comment.EscalationId);
        if (escalation is null)
            return Result.Failure(Error.NotFound<Escalation>(comment.EscalationId));
        
        if (comment.UserId != userId && escalation.AuthorId != userId)
            return Result.Failure(Error.Forbidden("Only the author or escalation author can delete the comment"));
        
        await _commentRepository.DeleteAsync(comment);
        return Result.Success();
    }
}