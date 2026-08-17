using Microsoft.AspNetCore.Mvc;
using Moq;
using UserManagement.Api.Controllers;
using UserManagement.Application.DTOs;
using UserManagement.Application.Exceptions;
using UserManagement.Application.Services;
using Xunit;

namespace UserManagement.UnitTests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();

        _controller = new UsersController(
            _userServiceMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithUsers()
    {
        // Arrange
        var users = new List<UserResponse>
        {
            new UserResponse(
                Guid.NewGuid(),
                "John",
                30,
                "mumbai",
                "maharashtra",
                "400001",
                DateTime.UtcNow),

            new UserResponse(
                Guid.NewGuid(),
                "David",
                35,
                "pune",
                "maharashtra",
                "411001",
                DateTime.UtcNow)
        };

        _userServiceMock
            .Setup(x => x.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _controller.GetAll(
            CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(
            result.Result);

        Assert.Equal(200, okResult.StatusCode);

        var returnedUsers =
            Assert.IsAssignableFrom<IReadOnlyList<UserResponse>>(
                okResult.Value);

        Assert.Equal(2, returnedUsers.Count);

        _userServiceMock.Verify(
            x => x.GetAllAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task GetAll_WhenNoUsers_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        var users = new List<UserResponse>();

        _userServiceMock
            .Setup(x => x.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _controller.GetAll(
            CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(
            result.Result);

        Assert.Equal(200, okResult.StatusCode);

        var returnedUsers =
            Assert.IsAssignableFrom<IReadOnlyList<UserResponse>>(
                okResult.Value);

        Assert.Empty(returnedUsers);
    }


    // =========================================================
    // GET BY ID
    // =========================================================

    [Fact]
    public async Task GetById_WhenUserExists_ShouldReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();

        var user = new UserResponse(
            id,
            "John",
            30,
            "mumbai",
            "maharashtra",
            "400001",
            DateTime.UtcNow);

        _userServiceMock
            .Setup(x => x.GetByIdAsync(
                id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.GetById(
            id,
            CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(
            result.Result);

        Assert.Equal(200, okResult.StatusCode);

        var returnedUser =
            Assert.IsType<UserResponse>(okResult.Value);

        Assert.Equal(id, returnedUser.Id);
        Assert.Equal("John", returnedUser.Name);
        Assert.Equal(30, returnedUser.Age);
        Assert.Equal("mumbai", returnedUser.City);
        Assert.Equal("maharashtra", returnedUser.State);
        Assert.Equal("400001", returnedUser.Pincode);

        _userServiceMock.Verify(
            x => x.GetByIdAsync(
                id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task GetById_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        _userServiceMock
            .Setup(x => x.GetByIdAsync(
                id,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new NotFoundException(
                    $"User '{id}' was not found."));

        // Act
        var result = await _controller.GetById(
            id,
            CancellationToken.None);

        // Assert
        var notFoundResult =
            Assert.IsType<NotFoundObjectResult>(result.Result);

        Assert.Equal(404, notFoundResult.StatusCode);

        var problemDetails =
            Assert.IsType<ProblemDetails>(
                notFoundResult.Value);

        Assert.Equal(
            "User not found",
            problemDetails.Title);

        Assert.Contains(
            id.ToString(),
            problemDetails.Detail);
    }


    // =========================================================
    // CREATE
    // =========================================================

    [Fact]
    public async Task Create_WhenSuccessful_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var id = Guid.NewGuid();

        var request = new CreateUserRequest
        {
            Name = "John",
            Age = 30,
            City = "Mumbai",
            State = "Maharashtra",
            Pincode = "400001"
        };

        var user = new UserResponse(
            id,
            "John",
            30,
            "mumbai",
            "maharashtra",
            "400001",
            DateTime.UtcNow);

        _userServiceMock
            .Setup(x => x.CreateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.Create(
            request,
            CancellationToken.None);

        // Assert
        var createdResult =
            Assert.IsType<CreatedAtActionResult>(
                result.Result);

        Assert.Equal(201, createdResult.StatusCode);

        Assert.Equal(
            nameof(UsersController.GetById),
            createdResult.ActionName);

        Assert.Equal(
            id,
            createdResult.RouteValues!["id"]);

        var returnedUser =
            Assert.IsType<UserResponse>(
                createdResult.Value);

        Assert.Equal(id, returnedUser.Id);
        Assert.Equal("John", returnedUser.Name);

        _userServiceMock.Verify(
            x => x.CreateAsync(
                request,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task Create_WhenDuplicateUser_ShouldReturnConflict()
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

        _userServiceMock
            .Setup(x => x.CreateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new ConflictException(
                    "User already exists."));

        // Act
        var result = await _controller.Create(
            request,
            CancellationToken.None);

        // Assert
        var conflictResult =
            Assert.IsType<ConflictObjectResult>(
                result.Result);

        Assert.Equal(409, conflictResult.StatusCode);

        var problemDetails =
            Assert.IsType<ProblemDetails>(
                conflictResult.Value);

        Assert.Equal(
            "Duplicate user",
            problemDetails.Title);

        Assert.Equal(
            "User already exists.",
            problemDetails.Detail);
    }


    // =========================================================
    // UPDATE
    // =========================================================

    [Fact]
    public async Task Update_WhenSuccessful_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var id = Guid.NewGuid();

        var request = new UpdateUserRequest
        {
            Name = "Updated John",
            Age = 31,
            City = "Pune",
            State = "Maharashtra",
            Pincode = "411001"
        };

        var updatedUser = new UserResponse(
            id,
            "Updated John",
            31,
            "Pune",
            "Maharashtra",
            "411001",
            DateTime.UtcNow);

        _userServiceMock
            .Setup(x => x.UpdateAsync(
                id,
                request,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _userServiceMock
            .Setup(x => x.GetByIdAsync(
                id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedUser);

        // Act
        var result = await _controller.Update(
            id,
            request,
            CancellationToken.None);

        // Assert
        var createdResult =
            Assert.IsType<CreatedAtActionResult>(
                result.Result);

        Assert.Equal(201, createdResult.StatusCode);

        Assert.Equal(
            nameof(UsersController.GetById),
            createdResult.ActionName);

        Assert.Equal(
            id,
            createdResult.RouteValues!["id"]);

        var returnedUser =
            Assert.IsType<UserResponse>(
                createdResult.Value);

        Assert.Equal(id, returnedUser.Id);
        Assert.Equal("Updated John", returnedUser.Name);
        Assert.Equal(31, returnedUser.Age);
        Assert.Equal("Pune", returnedUser.City);

        _userServiceMock.Verify(
            x => x.UpdateAsync(
                id,
                request,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _userServiceMock.Verify(
            x => x.GetByIdAsync(
                id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task Update_WhenUserDoesNotExist_ShouldReturnNotFound()
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

        _userServiceMock
            .Setup(x => x.UpdateAsync(
                id,
                request,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new NotFoundException(
                    $"User '{id}' was not found."));

        // Act
        var result = await _controller.Update(
            id,
            request,
            CancellationToken.None);

        // Assert
        var notFoundResult =
            Assert.IsType<NotFoundObjectResult>(
                result.Result);

        Assert.Equal(404, notFoundResult.StatusCode);

        var problemDetails =
            Assert.IsType<ProblemDetails>(
                notFoundResult.Value);

        Assert.Equal(
            "User not found",
            problemDetails.Title);

        Assert.Contains(
            id.ToString(),
            problemDetails.Detail);

        // GetById should never be called
        // because UpdateAsync failed.
        _userServiceMock.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }


    [Fact]
    public async Task Update_WhenDuplicateUser_ShouldReturnConflict()
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

        _userServiceMock
            .Setup(x => x.UpdateAsync(
                id,
                request,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new ConflictException(
                    "Duplicate user."));

        // Act
        var result = await _controller.Update(
            id,
            request,
            CancellationToken.None);

        // Assert
        var conflictResult =
            Assert.IsType<ConflictObjectResult>(
                result.Result);

        Assert.Equal(409, conflictResult.StatusCode);

        var problemDetails =
            Assert.IsType<ProblemDetails>(
                conflictResult.Value);

        Assert.Equal(
            "Duplicate user",
            problemDetails.Title);

        Assert.Equal(
            "Duplicate user.",
            problemDetails.Detail);
    }


    // =========================================================
    // DELETE
    // =========================================================

    [Fact]
    public async Task Delete_WhenSuccessful_ShouldReturnNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();

        _userServiceMock
            .Setup(x => x.DeleteAsync(
                id,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(
            id,
            CancellationToken.None);

        // Assert
        var noContentResult =
            Assert.IsType<NoContentResult>(result);

        Assert.Equal(
            204,
            noContentResult.StatusCode);

        _userServiceMock.Verify(
            x => x.DeleteAsync(
                id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task Delete_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        _userServiceMock
            .Setup(x => x.DeleteAsync(
                id,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new NotFoundException(
                    $"User '{id}' was not found."));

        // Act
        var result = await _controller.Delete(
            id,
            CancellationToken.None);

        // Assert
        var notFoundResult =
            Assert.IsType<NotFoundObjectResult>(
                result);

        Assert.Equal(404, notFoundResult.StatusCode);

        var problemDetails =
            Assert.IsType<ProblemDetails>(
                notFoundResult.Value);

        Assert.Equal(
            "User not found",
            problemDetails.Title);

        Assert.Contains(
            id.ToString(),
            problemDetails.Detail);

        _userServiceMock.Verify(
            x => x.DeleteAsync(
                id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}