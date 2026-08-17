using Moq;
using UserManagement.Application.Abstractions;
using UserManagement.Application.DTOs;
using UserManagement.Application.Exceptions;
using UserManagement.Application.Services;
using UserManagement.Domain.Entities;
using Xunit;

namespace UserManagement.UnitTests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _service = new UserService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var user1 = new User(
            "John",
            30,
            "Mumbai",
            "Maharashtra",
            "400001");

        var user2 = new User(
            "David",
            35,
            "Pune",
            "Maharashtra",
            "411001");

        var users = new List<User>
        {
            user1,
            user2
        };

        _repositoryMock
            .Setup(x => x.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _service.GetAllAsync(
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Equal(user1.Id, result[0].Id);
        Assert.Equal("John", result[0].Name);
        Assert.Equal(30, result[0].Age);
        Assert.Equal("mumbai", result[0].City);
        Assert.Equal("maharashtra", result[0].State);
        Assert.Equal("400001", result[0].Pincode);

        Assert.Equal(user2.Id, result[1].Id);
        Assert.Equal("David", result[1].Name);
        Assert.Equal(35, result[1].Age);
        Assert.Equal("pune", result[1].City);
        Assert.Equal("maharashtra", result[1].State);
        Assert.Equal("411001", result[1].Pincode);

        _repositoryMock.Verify(
            x => x.GetAllAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoUsers_ShouldReturnEmptyList()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _service.GetAllAsync(
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _repositoryMock.Verify(
            x => x.GetAllAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var user = new User(
            "John",
            30,
            "Mumbai",
            "Maharashtra",
            "400001");

        _repositoryMock
            .Setup(x => x.GetByIdAsync(
                user.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.GetByIdAsync(
            user.Id,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(user.Id, result.Id);
        Assert.Equal("John", result.Name);
        Assert.Equal(30, result.Age);
        Assert.Equal("mumbai", result.City);
        Assert.Equal("maharashtra", result.State);
        Assert.Equal("400001", result.Pincode);
        Assert.Equal(user.CreatedAtUtc, result.CreatedAtUtc);

        _repositoryMock.Verify(
            x => x.GetByIdAsync(
                user.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(x => x.GetByIdAsync(
                id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetByIdAsync(
                id,
                CancellationToken.None));

        Assert.Contains(
            id.ToString(),
            exception.Message);

        _repositoryMock.Verify(
            x => x.GetByIdAsync(
                id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateUser()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Name = "John",
            Age = 30,
            City = "Mumbai",
            State = "Maharashtra",
            Pincode = "400001"
        };

        _repositoryMock
            .Setup(x => x.AddAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(
            request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("John", result.Name);
        Assert.Equal(30, result.Age);
        Assert.Equal("mumbai", result.City);
        Assert.Equal("maharashtra", result.State);
        Assert.Equal("400001", result.Pincode);

        _repositoryMock.Verify(
            x => x.AddAsync(
                It.Is<User>(u =>
                    u.Name == "John" &&
                    u.Age == 30 &&
                    u.City == "mumbai" &&
                    u.State == "maharashtra" &&
                    u.Pincode == "400001"),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _repositoryMock.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserExists_ShouldUpdateUser()
    {
        // Arrange
        var user = new User(
            "Old Name",
            25,
            "Mumbai",
            "Maharashtra",
            "400001");

        var request = new UpdateUserRequest
        {
            Name = "New Name",
            Age = 30,
            City = "Pune",
            State = "Maharashtra",
            Pincode = "411001"
        };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(
                user.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _repositoryMock
            .Setup(x => x.UpdateAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(
            user.Id,
            request,
            CancellationToken.None);

        // Assert
        Assert.Equal("New Name", user.Name);
        Assert.Equal(30, user.Age);
        Assert.Equal("Pune", user.City);
        Assert.Equal("Maharashtra", user.State);
        Assert.Equal("411001", user.Pincode);

        _repositoryMock.Verify(
            x => x.GetByIdAsync(
                user.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _repositoryMock.Verify(
            x => x.UpdateAsync(
                user,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        var request = new UpdateUserRequest
        {
            Name = "John",
            Age = 30,
            City = "Mumbai",
            State = "Maharashtra",
            Pincode = "400001"
        };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(
                id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.UpdateAsync(
                id,
                request,
                CancellationToken.None));

        Assert.Contains(
            "John",
            exception.Message);

        _repositoryMock.Verify(
            x => x.UpdateAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(x => x.GetByIdAsync(
                id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.DeleteAsync(
                id,
                CancellationToken.None));

        Assert.Contains(
            id.ToString(),
            exception.Message);

        _repositoryMock.Verify(
            x => x.DeleteAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _repositoryMock.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}