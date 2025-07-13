using FluentAssertions;
using Models.DTOs;
using NSubstitute;
using UserService.Application.Common.Interfaces.Repository;
using UserService.Domain;
using Xunit;

namespace UserService.Application.Tests;

public class UserServiceTests
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly Services.Implementation.UserService _userService;
    
    public UserServiceTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _userService = new Services.Implementation.UserService(_userRepositoryMock);
    }
    
    [Fact]
    public async Task CheckUsersExist_ShouldReturnFalse_WhenUserIdsIsNull()
    {
        // Act
        var result = await _userService.CheckUsersExist(null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CheckUsersExist_ShouldReturnFalse_WhenUserIdsIsEmpty()
    {
        // Act
        var result = await _userService.CheckUsersExist(new List<int>());

        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task CheckUsersExist_ShouldReturnFalse_WhenNotAllUsersExist()
    {
        // Arrange
        var userIds = new List<int> { 1, 2, 3 };
        _userRepositoryMock.CheckUsersExistAsync(userIds).Returns(new List<int> { 1, 3 });

        // Act
        var result = await _userService.CheckUsersExist(userIds);

        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task CheckUsersExist_ShouldReturnTrue_WhenAllUsersExist()
    {
        // Arrange
        var userIds = new List<int> { 1, 2, 3 };
        _userRepositoryMock.CheckUsersExistAsync(userIds).Returns(new List<int> { 1, 2, 3 });

        // Act
        var result = await _userService.CheckUsersExist(userIds);

        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task GetUsersInfo_ShouldReturnUsersInfo_WhenUsersExist()
    {
        // Arrange
        var userIds = new List<int> { 1, 2 };
        var users = new List<User>
        {
            new() { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" },
            new() { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" }
        };
        
        _userRepositoryMock.GetUsersByIdsAsync(userIds).Returns(users);
        
        var expectedResult = users.Select(u => new UserDto(u.Id, u.FirstName, u.LastName, u.Email)).ToList();

        // Act
        var result = await _userService.GetUsersInfo(userIds);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
    
    [Fact]
    public async Task GetUsersInfo_ShouldReturnEmptyList_WhenNoUsersFound()
    {
        // Arrange
        var userIds = new List<int> { 1, 2 };
        _userRepositoryMock.GetUsersByIdsAsync(userIds).Returns(new List<User>());

        // Act
        var result = await _userService.GetUsersInfo(userIds);

        // Assert
        result.Should().BeEmpty();
    }
}