using AutoMapper;
using EscalationService.Appliacation.Common.Interfaces;
using EscalationService.Appliacation.DTOs;
using EscalationService.Appliacation.Services.Implementation;
using EscalationService.Domain.Entities;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Models.Result;
using NSubstitute;
using Xunit;

namespace EscalationService.Appliacation.Tests;

public class CommentServiceTests
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IValidator<CommentDto> _validatorMock;
    private readonly IUserContext _userContextMock;
    private readonly IMapper _mapper;
    private readonly CommentService _commentService;

    public CommentServiceTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _validatorMock = Substitute.For<IValidator<CommentDto>>();
        _userContextMock = Substitute.For<IUserContext>();

        var config = new MapperConfiguration(cfg => { cfg.CreateMap<CommentDto, Comment>(); });
        _mapper = config.CreateMapper();

        _commentService = new CommentService(
            _unitOfWorkMock,
            _validatorMock,
            _userContextMock,
            _mapper);
    }

    [Fact]
    public async Task GetByEscalationIdAsync_ShouldReturnValidationError_WhenIdIsZero()
    {
        // Act
        var result = await _commentService.GetByEscalationIdAsync(0);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.ValidationFailed("Escalation ID must be a positive number"));
    }

    [Fact]
    public async Task GetByEscalationIdAsync_ShouldReturnNotFound_WhenEscalationNotExists()
    {
        // Arrange
        const int escalationId = 1;
        _unitOfWorkMock.Escalations.ExistsAsync(escalationId).Returns(false);

        // Act
        var result = await _commentService.GetByEscalationIdAsync(escalationId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.NotFound<Escalation>(escalationId));
    }

    [Fact]
    public async Task GetByEscalationIdAsync_ShouldReturnComments_WhenEscalationExists()
    {
        // Arrange
        const int escalationId = 1;
        var expectedComments = new List<Comment> { new Comment { Id = 1 } };

        _unitOfWorkMock.Escalations.ExistsAsync(escalationId).Returns(true);
        _unitOfWorkMock.Comments.GetByEscalationIdAsync(escalationId).Returns(expectedComments);

        // Act
        var result = await _commentService.GetByEscalationIdAsync(escalationId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(expectedComments);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnValidationError_WhenIdIsZero()
    {
        // Act
        var result = await _commentService.GetByUserIdAsync(0);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.ValidationFailed("Escalation ID must be a positive number"));
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnComments()
    {
        // Arrange
        const int userId = 1;
        var expectedComments = new List<Comment> { new Comment { Id = 1 } };

        _unitOfWorkMock.Comments.GetByUserIdAsync(userId).Returns(expectedComments);

        // Act
        var result = await _commentService.GetByUserIdAsync(userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(expectedComments);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnValidationError_WhenDtoIsInvalid()
    {
        // Arrange
        var dto = new CommentDto(Text: "");
        var validationResult = new ValidationResult(new[] { new ValidationFailure("Text", "Text is required") });
        _validatorMock.ValidateAsync(dto).Returns(validationResult);

        // Act
        var result = await _commentService.CreateAsync(dto, 1);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.ValidationFailed("Text is required"));
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnNotFound_WhenEscalationNotExists()
    {
        // Arrange
        const int escalationId = 1;
        var dto = new CommentDto(Text: "Test");
        _validatorMock.ValidateAsync(dto).Returns(new ValidationResult());
        _unitOfWorkMock.Escalations.GetByIdAsync(escalationId).Returns((Escalation)null);

        // Act
        var result = await _commentService.CreateAsync(dto, escalationId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.NotFound<Escalation>(escalationId));
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateComment_WhenDataIsValid()
    {
        // Arrange
        const int escalationId = 1;
        const int userId = 100;
        var dto = new CommentDto(Text: "Test");
        var escalation = new Escalation { Id = escalationId };

        _validatorMock.ValidateAsync(dto).Returns(new ValidationResult());
        _unitOfWorkMock.Escalations.GetByIdAsync(escalationId).Returns(escalation);
        _userContextMock.GetUserId().Returns(userId);
        _unitOfWorkMock.Comments.AddAsync(Arg.Any<Comment>()).Returns(Task.CompletedTask);
        _unitOfWorkMock.SaveChangesAsync().Returns(Task.FromResult(1));

        // Act
        var result = await _commentService.CreateAsync(dto, escalationId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Text.Should().Be("Test");
        result.Data.EscalationId.Should().Be(escalationId);
        result.Data.UserId.Should().Be(userId);

        await _unitOfWorkMock.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnValidationError_WhenDtoIsInvalid()
    {
        // Arrange
        const int commentId = 1;
        var dto = new CommentDto(Text: "");
        var validationResult = new ValidationResult(new[] { new ValidationFailure("Text", "Text is required") });
        _validatorMock.ValidateAsync(dto).Returns(validationResult);

        // Act
        var result = await _commentService.UpdateAsync(commentId, dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.ValidationFailed("Text is required"));
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNotFound_WhenCommentNotExists()
    {
        // Arrange
        const int commentId = 1;
        var dto = new CommentDto(Text: "Updated");
        _validatorMock.ValidateAsync(dto).Returns(new ValidationResult());
        _unitOfWorkMock.Comments.GetByIdAsync(commentId).Returns((Comment)null);

        // Act
        var result = await _commentService.UpdateAsync(commentId, dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.NotFound<Comment>(commentId));
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnForbidden_WhenUserIsNotAuthorAndNotSenior()
    {
        // Arrange
        const int commentId = 1;
        const int authorId = 100;
        var dto = new CommentDto(Text: "Updated");
        var comment = new Comment { Id = commentId, UserId = authorId };

        _validatorMock.ValidateAsync(dto).Returns(new ValidationResult());
        _unitOfWorkMock.Comments.GetByIdAsync(commentId).Returns(comment);
        _userContextMock.GetUserId().Returns(200); 
        _userContextMock.GetUserRole().Returns("Junior"); 

        // Act
        var result = await _commentService.UpdateAsync(commentId, dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.Forbidden("You can only edit your own comments"));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateComment_WhenUserIsAuthor()
    {
        // Arrange
        const int commentId = 1;
        const int userId = 100;
        var dto = new CommentDto(Text: "Updated");
        var comment = new Comment { Id = commentId, UserId = userId, Text = "Original" };

        _validatorMock.ValidateAsync(dto).Returns(new ValidationResult());
        _unitOfWorkMock.Comments.GetByIdAsync(commentId).Returns(comment);
        _userContextMock.GetUserId().Returns(userId);
        _userContextMock.GetUserRole().Returns("Middle");
        _unitOfWorkMock.Comments.UpdateAsync(Arg.Any<Comment>()).Returns(Task.CompletedTask);
        _unitOfWorkMock.SaveChangesAsync().Returns(Task.FromResult(1));

        // Act
        var result = await _commentService.UpdateAsync(commentId, dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Text.Should().Be("Updated");
        await _unitOfWorkMock.Received(1).SaveChangesAsync();
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldReturnForbidden_WhenUserIsNotAuthorAndNotSenior()
    {
        // Arrange
        const int commentId = 1;
        const int authorId = 100;
        var comment = new Comment { Id = commentId, UserId = authorId };

        _unitOfWorkMock.Comments.GetByIdAsync(commentId).Returns(comment);
        _userContextMock.GetUserId().Returns(200); 
        _userContextMock.GetUserRole().Returns("Junior");

        // Act
        var result = await _commentService.DeleteAsync(commentId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.Forbidden("You can only delete your own comments"));
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldDeleteComment_WhenUserIsAuthor()
    {
        // Arrange
        const int commentId = 1;
        const int userId = 100;
        var comment = new Comment { Id = commentId, UserId = userId };

        _unitOfWorkMock.Comments.GetByIdAsync(commentId).Returns(comment);
        _userContextMock.GetUserId().Returns(userId);
        _userContextMock.GetUserRole().Returns("Middle");
        _unitOfWorkMock.Comments.DeleteAsync(comment).Returns(Task.CompletedTask);
        _unitOfWorkMock.SaveChangesAsync().Returns(Task.FromResult(1));

        // Act
        var result = await _commentService.DeleteAsync(commentId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _unitOfWorkMock.Received(1).SaveChangesAsync();
    }
}

