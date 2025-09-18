using AutoMapper;
using EscalationService.Appliacation.Common.Interfaces;
using EscalationService.Appliacation.Models.DTOs.Criteria;
using EscalationService.Appliacation.Services.Implementation;
using EscalationService.Domain.Entities;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Models.Result;
using NSubstitute;
using Xunit;

namespace EscalationService.Appliacation.Tests;

public class CriteriaServiceTests
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IValidator<CreateCriteriaDto> _createValidatorMock;
    private readonly IValidator<UpdateCriteriaDto> _updateValidatorMock;
    private readonly IUserContext _userContextMock;
    private readonly IMapper _mapper;
    private readonly CriteriaService _criteriaService;
    
    public CriteriaServiceTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _createValidatorMock = Substitute.For<IValidator<CreateCriteriaDto>>();
        _updateValidatorMock = Substitute.For<IValidator<UpdateCriteriaDto>>();
        _userContextMock = Substitute.For<IUserContext>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateCriteriaDto, Criteria>();
            cfg.CreateMap<UpdateCriteriaDto, Criteria>();
        });
        _mapper = config.CreateMapper();

        _criteriaService = new CriteriaService(
            _unitOfWorkMock,
            _createValidatorMock,
            _updateValidatorMock,
            _userContextMock,
            _mapper);
    }
    
    [Fact]
    public async Task GetByEscalationIdAsync_ShouldReturnValidationError_WhenIdIsZero()
    {
        // Act
        var result = await _criteriaService.GetByEscalationIdAsync(0);

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
        var result = await _criteriaService.GetByEscalationIdAsync(escalationId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.NotFound<Escalation>(escalationId));
    }
    
    [Fact]
    public async Task GetByEscalationIdAsync_ShouldReturnCriteriaList_WhenEscalationExists()
    {
        // Arrange
        const int escalationId = 1;
        var expectedCriteria = new List<Criteria> { new Criteria { Id = 1 } };

        _unitOfWorkMock.Escalations.ExistsAsync(escalationId).Returns(true);
        _unitOfWorkMock.Criterias.GetByEscalationIdAsync(escalationId).Returns(expectedCriteria);

        // Act
        var result = await _criteriaService.GetByEscalationIdAsync(escalationId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(expectedCriteria);
    }
    
    [Fact]
    public async Task CreateAsync_ShouldReturnValidationError_WhenDtoIsInvalid()
    {
        // Arrange
        var dto = new CreateCriteriaDto(Description:"test");
        var validationResult = new ValidationResult(new[] { new ValidationFailure("Description", "Description is required") });
        _createValidatorMock.ValidateAsync(dto).Returns(validationResult);

        // Act
        var result = await _criteriaService.CreateAsync(1, dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.ValidationFailed("Description is required"));
    }
    
    [Fact]
    public async Task CreateAsync_ShouldReturnNotFound_WhenEscalationNotExists()
    {
        // Arrange
        const int escalationId = 1;
        var dto = new CreateCriteriaDto(Description: "Test");
        _createValidatorMock.ValidateAsync(dto).Returns(new ValidationResult());
        _unitOfWorkMock.Escalations.GetByIdAsync(escalationId).Returns((Escalation)null);

        // Act
        var result = await _criteriaService.CreateAsync(escalationId, dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.NotFound<Escalation>(escalationId));
    }
    
    [Fact]
    public async Task CreateAsync_ShouldReturnForbidden_WhenUserHasNoPermission()
    {
        // Arrange
        const int escalationId = 1;
        var dto = new CreateCriteriaDto(Description: "Test");
        var escalation = new Escalation { Id = escalationId, AuthorId = 100 };

        _createValidatorMock.ValidateAsync(dto).Returns(new ValidationResult());
        _unitOfWorkMock.Escalations.GetByIdAsync(escalationId).Returns(escalation);
        _userContextMock.GetUserId().Returns(200);
        _userContextMock.GetUserRole().Returns("Junior");
        _unitOfWorkMock.Criterias.CountByEscalationIdAsync(escalationId).Returns(0);

        // Act
        var result = await _criteriaService.CreateAsync(escalationId, dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.Forbidden("No permissions to modify this escalation"));
    }
    
    [Fact]
    public async Task CreateAsync_ShouldCreateCriteria_WhenDataIsValid()
    {
        // Arrange
        const int escalationId = 1;
        var dto = new CreateCriteriaDto(Description: "Test");
        var escalation = new Escalation { Id = escalationId, AuthorId = 100 };

        _createValidatorMock.ValidateAsync(dto).Returns(new ValidationResult());
        _unitOfWorkMock.Escalations.GetByIdAsync(escalationId).Returns(escalation);
        _userContextMock.GetUserId().Returns(100);
        _userContextMock.GetUserRole().Returns("Middle");
        _unitOfWorkMock.Criterias.CountByEscalationIdAsync(escalationId).Returns(0);
        _unitOfWorkMock.Criterias.AddAsync(Arg.Any<Criteria>()).Returns(Task.CompletedTask);
        _unitOfWorkMock.SaveChangesAsync().Returns(Task.FromResult(1));

        // Act
        var result = await _criteriaService.CreateAsync(escalationId, dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();

        result.Data.Description.Should().Be("Test");
        result.Data.Order.Should().Be(1);
        result.Data.EscalationId.Should().Be(escalationId);
        result.Data.IsCompleted.Should().BeFalse();

        await _unitOfWorkMock.Received(1).SaveChangesAsync();
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldUpdateAllProperties_WhenDataIsValid()
    {
        // Arrange
        const int criteriaId = 1;
        const int escalationId = 10;
        var dto = new UpdateCriteriaDto(
            Description: "Updated Description", 
            IsCompleted: true, 
            Order: 2);
    
        var existingCriteria = new Criteria 
        { 
            Id = criteriaId, 
            Description = "Old Description",
            IsCompleted = false,
            Order = 1,
            EscalationId = escalationId
        };
    
        var escalation = new Escalation { Id = escalationId, AuthorId = 100 };

        _updateValidatorMock.ValidateAsync(dto).Returns(new ValidationResult());
        _unitOfWorkMock.Criterias.GetByIdAsync(criteriaId).Returns(existingCriteria);
        _unitOfWorkMock.Escalations.GetByIdAsync(escalationId).Returns(escalation);
        _userContextMock.GetUserId().Returns(100); 
        _userContextMock.GetUserRole().Returns("Middle");
        _unitOfWorkMock.Criterias.UpdateAsync(Arg.Any<Criteria>()).Returns(Task.CompletedTask);
        _unitOfWorkMock.SaveChangesAsync().Returns(Task.FromResult(1));

        // Act
        var result = await _criteriaService.UpdateAsync(criteriaId, dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    
        result.Data.Description.Should().Be(dto.Description);
        result.Data.IsCompleted.Should().Be(dto.IsCompleted);
        result.Data.Order.Should().Be(dto.Order);
    
        result.Data.Id.Should().Be(criteriaId);
        result.Data.EscalationId.Should().Be(escalationId);
    
        await _unitOfWorkMock.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateAsync_ShouldAllowSeniorToModifyAnyEscalation()
    {
        // У автора эскалации и пользователя сейчас разные id, но пользователь - сеньор
        
        // Arrange
        const int criteriaId = 1;
        const int escalationId = 10;
        var dto = new UpdateCriteriaDto("Updated", true, 2);
        var criteria = new Criteria { Id = criteriaId, EscalationId = escalationId };
        var escalation = new Escalation { Id = escalationId, AuthorId = 999 }; 

        _updateValidatorMock.ValidateAsync(dto).Returns(new ValidationResult());
        _unitOfWorkMock.Criterias.GetByIdAsync(criteriaId).Returns(criteria);
        _unitOfWorkMock.Escalations.GetByIdAsync(escalationId).Returns(escalation);
        _userContextMock.GetUserId().Returns(100); 
        _userContextMock.GetUserRole().Returns("Senior"); 
        _unitOfWorkMock.Criterias.UpdateAsync(Arg.Any<Criteria>()).Returns(Task.CompletedTask);
        _unitOfWorkMock.SaveChangesAsync().Returns(Task.FromResult(1));

        // Act
        var result = await _criteriaService.UpdateAsync(criteriaId, dto);

        // Assert
        result.IsSuccess.Should().BeTrue(); 
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldReturnNotFound_WhenCriteriaNotExists()
    {
        // Arrange
        const int criteriaId = 1;
        _unitOfWorkMock.Criterias.GetByIdAsync(criteriaId).Returns((Criteria)null);

        // Act
        var result = await _criteriaService.DeleteAsync(criteriaId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.NotFound<Criteria>(criteriaId));
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldReturnForbidden_WhenUserHasNoPermission()
    {
        // Arrange
        const int criteriaId = 1;
        const int escalationId = 10;
        var criteria = new Criteria { Id = criteriaId, EscalationId = escalationId };
        var escalation = new Escalation { Id = escalationId, AuthorId = 100 };

        _unitOfWorkMock.Criterias.GetByIdAsync(criteriaId).Returns(criteria);
        _unitOfWorkMock.Escalations.GetByIdAsync(escalationId).Returns(escalation);
        _userContextMock.GetUserId().Returns(200); 
        _userContextMock.GetUserRole().Returns("Junior"); 

        // Act
        var result = await _criteriaService.DeleteAsync(criteriaId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.Forbidden("No permissions to modify this escalation"));
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldDeleteCriteria_WhenUserIsAuthor()
    {
        // Arrange
        const int criteriaId = 1;
        const int escalationId = 10;
        var criteria = new Criteria { Id = criteriaId, EscalationId = escalationId };
        var escalation = new Escalation { Id = escalationId, AuthorId = 100 };

        _unitOfWorkMock.Criterias.GetByIdAsync(criteriaId).Returns(criteria);
        _unitOfWorkMock.Escalations.GetByIdAsync(escalationId).Returns(escalation);
        _userContextMock.GetUserId().Returns(100);
        _userContextMock.GetUserRole().Returns("Middle");
        _unitOfWorkMock.Criterias.DeleteAsync(criteria).Returns(Task.CompletedTask);
        _unitOfWorkMock.SaveChangesAsync().Returns(Task.FromResult(1));

        // Act
        var result = await _criteriaService.DeleteAsync(criteriaId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _unitOfWorkMock.Received(1).SaveChangesAsync();
    }
}