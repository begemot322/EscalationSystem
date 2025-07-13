using System.Linq.Expressions;
using AutoMapper;
using EscalationService.Appliacation.Common.Interfaces;
using EscalationService.Appliacation.DTOs;
using EscalationService.Appliacation.Filters;
using EscalationService.Domain.Entities;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Models;
using Models.DTOs;
using Models.QueryParams;
using Models.Result;
using NSubstitute;
using Xunit;

namespace EscalationService.Appliacation.Tests;

public class EscalationServiceTests
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IValidator<EscalationDto> _validatorMock;
    private readonly IUserServiceClient _userServiceClientMock;
    private readonly IUserContext _userContextMock;
    private readonly IMessageBusPublisher _messageBusPublisherMock;
    private readonly IMapper _mapper;
    private readonly Services.Implementation.EscalationService _escalationService;

    public EscalationServiceTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _validatorMock = Substitute.For<IValidator<EscalationDto>>();
        _userServiceClientMock = Substitute.For<IUserServiceClient>();
        _userContextMock = Substitute.For<IUserContext>();
        _messageBusPublisherMock = Substitute.For<IMessageBusPublisher>();
        
        var config = new MapperConfiguration(cfg => 
        {
            cfg.CreateMap<EscalationDto, Escalation>();
            cfg.CreateMap<Escalation, EscalationDtoMessage>();
            cfg.CreateMap<Escalation, EscalationReminderDto>();
        });
        _mapper = config.CreateMapper();
        
        _escalationService = new Services.Implementation.EscalationService(
            _unitOfWorkMock,
            _validatorMock,
            _userServiceClientMock,
            _userContextMock,
            _messageBusPublisherMock,
            _mapper);
    }

    [Fact]
    public async Task GetAllEscalationsAsync_ShouldReturnPagedResult_WhenCalled()
    {
        // Arrange - подготовка
        var expected = new PagedResult<Escalation>(new List<Escalation> { new Escalation() }, 1, 1, 10);
        _unitOfWorkMock.Escalations.GetAllAsync(Arg.Any<EscalationFilter>(), Arg.Any<SortParams>(), Arg.Any<PageParams>())
            .Returns(expected);
        
        // Act - действие
        var result = await _escalationService.GetAllEscalationsAsync();

        // Assert - проверка
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task GetEscalationByIdAsync_ShouldReturnValidationError_WhenIdIsZero()
    {
        // Act
        var result = await _escalationService.GetEscalationByIdAsync(0);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.ValidationFailed("ID must be a positive number"));
    }
    
    [Fact]
    public async Task GetEscalationByIdAsync_ShouldReturnNotFound_WhenEscalationNotExists()
    {
        // Arrange
        const int testId = 1;
        _unitOfWorkMock.Escalations.GetByIdAsync(testId).Returns((Escalation)null);

        // Act
        var result = await _escalationService.GetEscalationByIdAsync(testId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.NotFound<Escalation>(testId));
    }
    
    [Fact]
    public async Task GetEscalationByIdAsync_ShouldReturnEscalation_WhenExists()
    {
        // Arrange
        var expected = new Escalation { Id = 1 };
        _unitOfWorkMock.Escalations.GetByIdAsync(1).Returns(expected);

        // Act
        var result = await _escalationService.GetEscalationByIdAsync(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task CreateEscalationAsync_ShouldReturnForbidden_WhenUserIsJunior()
    {
        // Arrange
        var dto = new EscalationDto(Name: "Test Escalation",
            Description: "Test Description",
            ResponsibleUserIds: new List<int> { 1, 2, 3 }, 
            Status: EscalationStatus.New);
        
        _userContextMock.GetUserRole().Returns("Junior");

        // Act
        var result = await _escalationService.CreateEscalationAsync(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.Forbidden("Only Middle and Senior levels can create escalations"));
    }
    
    [Fact]
    public async Task CreateEscalationAsync_ShouldReturnValidationError_WhenDtoInvalid()
    {
        // Arrange
        var dto = new EscalationDto(Name: "Test Escalation",
            Description: "Test Description",
            ResponsibleUserIds: new List<int> { 1, 2, 3 }, 
            Status: EscalationStatus.New);
        
        var validationResult = new ValidationResult(new[] { new ValidationFailure("Title", "Title is required") });
        _validatorMock.ValidateAsync(dto).Returns(validationResult);
        _userContextMock.GetUserRole().Returns("Middle");

        // Act
        var result = await _escalationService.CreateEscalationAsync(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.ValidationFailed("Title is required"));
    }
    
    [Fact]
    public async Task CreateEscalationAsync_ShouldReturnNotFound_WhenUsersNotExist()
    {
        // Arrange
        var dto = new EscalationDto(Name: "Test Escalation",
            Description: "Test Description",
            ResponsibleUserIds: new List<int> { 1, 2, 3 }, 
            Status: EscalationStatus.New);
        
        _validatorMock.ValidateAsync(dto).Returns(new ValidationResult());
        _userServiceClientMock.CheckUsersExistAsync(new List<int> { 1, 2, 3 }).Returns(false);
        _userContextMock.GetUserRole().Returns("Middle");

        // Act
        var result = await _escalationService.CreateEscalationAsync(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.NotFound("User", "one or more responsible users not found"));
    }
    
    [Fact]
    public async Task UpdateEscalationAsync_ShouldReturnForbidden_WhenNotAuthorOrSenior()
    {
        // Arrange
        var dto = new EscalationDto(Name: "Test Escalation",
            Description: "Test Description",
            ResponsibleUserIds: new List<int> { 1, 2, 3 }, 
            Status: EscalationStatus.New);
        
        var existing = new Escalation { Id = 1, AuthorId = 100 };
        _unitOfWorkMock.Escalations.GetByIdAsync(1).Returns(existing);
        _userContextMock.GetUserId().Returns(200);
        _userContextMock.GetUserRole().Returns("Junior");

        // Act
        var result = await _escalationService.UpdateEscalationAsync(1, dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.Forbidden("Only creator or Senior can update"));
    }
    
    [Fact]
    public async Task UpdateEscalationAsync_ShouldUpdateEscalation_WhenAuthor()
    {
        // Arrange
        var dto = new EscalationDto(Name: "Test name",
            Description: "Test Description",
            ResponsibleUserIds: new List<int> { 1, 2, 3 },
            Status: EscalationStatus.New);
        var existing = new Escalation { Id = 1, AuthorId = 123, Name = "Original" };
        
        _unitOfWorkMock.Escalations.GetByIdAsync(1).Returns(existing);
        _validatorMock.ValidateAsync(dto).Returns(new ValidationResult());
        _userContextMock.GetUserId().Returns(123);
        _userContextMock.GetUserRole().Returns("Middle");
        _userServiceClientMock.CheckUsersExistAsync(dto.ResponsibleUserIds).Returns(true);


        // Act
        var result = await _escalationService.UpdateEscalationAsync(1, dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Name.Should().Be("Test name");
        await _unitOfWorkMock.Received(1).SaveChangesAsync();
    }
    
    [Fact]
    public async Task DeleteEscalationAsync_ShouldReturnForbidden_WhenNotSenior()
    {
        // Arrange
        _userContextMock.GetUserRole().Returns("Middle");

        // Act
        var result = await _escalationService.DeleteEscalationAsync(1);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.Forbidden("Only Senior can delete escalations"));
    }
    
    [Fact]
    public async Task DeleteEscalationAsync_ShouldDelete_WhenSenior()
    {
        // Arrange
        var escalation = new Escalation { Id = 1 };
        _userContextMock.GetUserRole().Returns("Senior");
        _unitOfWorkMock.Escalations.ExistsAsync(1).Returns(true);
        _unitOfWorkMock.Escalations.GetByIdAsync(1).Returns(escalation);

        // Act
        var result = await _escalationService.DeleteEscalationAsync(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _unitOfWorkMock.Escalations.Received(1).DeleteAsync(escalation);
    }
    
    [Fact]
    public async Task GetOverdueEscalationsAsync_ShouldReturnOverdueEscalations()
    {
        // Arrange
        var overdueEscalation = new Escalation 
        { 
            Id = 1,
            CreatedAt = DateTime.Now.AddDays(-31),
            Status = EscalationStatus.InProgress
        };
        
        _unitOfWorkMock.Escalations.GetByExpressionAsync(
                Arg.Any<Expression<Func<Escalation, bool>>>(),
                Arg.Any<string>(),
                Arg.Any<bool>())
            .Returns(new List<Escalation> { overdueEscalation });

        // Act
        var result = await _escalationService.GetOverdueEscalationsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().ContainSingle();
        result.Data[0].Id.Should().Be(1);
    }
}