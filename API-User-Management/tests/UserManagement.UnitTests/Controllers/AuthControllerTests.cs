using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UserManagement.Api.Controllers;
using Xunit;

namespace UserManagement.UnitTests.Controllers;

public class AuthControllerTests
{
    private readonly IConfiguration _configuration;
    private readonly AuthController _controller;

    private const string ClientId = "test-client";
    private const string ClientSecret = "test-secret";
    private const string JwtKey =
        "ThisIsASecretKeyForUnitTesting123456789";
    private const string JwtIssuer = "UserManagement";
    private const string JwtAudience = "UserManagementClient";
    private const string JwtExpiryMinutes = "60";

    public AuthControllerTests()
    {
        var configurationData = new Dictionary<string, string?>
        {
            ["Auth:ClientID"] = ClientId,
            ["Auth:ClientSecret"] = ClientSecret,

            ["Jwt:Key"] = JwtKey,
            ["Jwt:Issuer"] = JwtIssuer,
            ["Jwt:Audience"] = JwtAudience,
            ["Jwt:ExpiryMinutes"] = JwtExpiryMinutes
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();

        _controller = new AuthController(_configuration);
    }


    // =========================================================
    // Login - Success
    // =========================================================

    [Fact]
    public void Login_WithValidCredentials_ShouldReturnOk()
    {
        // Arrange
        var request = new LoginRequest
        {
            ClientId = ClientId,
            ClientSecret = ClientSecret
        };

        // Act
        var result = _controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, okResult.StatusCode);
        Assert.NotNull(okResult.Value);
    }


    [Fact]
    public void Login_WithValidCredentials_ShouldReturnJwtToken()
    {
        // Arrange
        var request = new LoginRequest
        {
            ClientId = ClientId,
            ClientSecret = ClientSecret
        };

        // Act
        var result = _controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.NotNull(okResult.Value);

        var tokenProperty = okResult.Value
            .GetType()
            .GetProperty("token");

        Assert.NotNull(tokenProperty);

        var token = tokenProperty!
            .GetValue(okResult.Value)?
            .ToString();

        Assert.False(string.IsNullOrWhiteSpace(token));

        // Verify that the returned value is a valid JWT
        var handler = new JwtSecurityTokenHandler();

        Assert.True(
            handler.CanReadToken(token));
    }


    // =========================================================
    // Login - Invalid ClientId
    // =========================================================

    [Fact]
    public void Login_WithInvalidClientId_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            ClientId = "invalid-client",
            ClientSecret = ClientSecret
        };

        // Act
        var result = _controller.Login(request);

        // Assert
        var unauthorizedResult =
            Assert.IsType<UnauthorizedObjectResult>(result);

        Assert.Equal(
            401,
            unauthorizedResult.StatusCode);

        Assert.Equal(
            "Invalid email or password",
            unauthorizedResult.Value);
    }


    // =========================================================
    // Login - Invalid ClientSecret
    // =========================================================

    [Fact]
    public void Login_WithInvalidClientSecret_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            ClientId = ClientId,
            ClientSecret = "invalid-secret"
        };

        // Act
        var result = _controller.Login(request);

        // Assert
        var unauthorizedResult =
            Assert.IsType<UnauthorizedObjectResult>(result);

        Assert.Equal(
            401,
            unauthorizedResult.StatusCode);

        Assert.Equal(
            "Invalid email or password",
            unauthorizedResult.Value);
    }


    // =========================================================
    // Login - Both credentials invalid
    // =========================================================

    [Fact]
    public void Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            ClientId = "invalid-client",
            ClientSecret = "invalid-secret"
        };

        // Act
        var result = _controller.Login(request);

        // Assert
        var unauthorizedResult =
            Assert.IsType<UnauthorizedObjectResult>(result);

        Assert.Equal(
            401,
            unauthorizedResult.StatusCode);

        Assert.Equal(
            "Invalid email or password",
            unauthorizedResult.Value);
    }


    // =========================================================
    // JWT Claims
    // =========================================================

    [Fact]
    public void Login_ShouldGenerateTokenWithExpectedClaims()
    {
        // Arrange
        var request = new LoginRequest
        {
            ClientId = ClientId,
            ClientSecret = ClientSecret
        };

        // Act
        var result = _controller.Login(request);

        var okResult = Assert.IsType<OkObjectResult>(result);

        var tokenProperty = okResult.Value!
            .GetType()
            .GetProperty("token");

        var tokenString = tokenProperty!
            .GetValue(okResult.Value)?
            .ToString();

        Assert.False(string.IsNullOrWhiteSpace(tokenString));

        // Read JWT
        var handler = new JwtSecurityTokenHandler();

        var token = handler.ReadJwtToken(tokenString);

        // Assert
        var nameIdentifier = token.Claims
            .FirstOrDefault(
                x => x.Type == ClaimTypes.NameIdentifier);

        var email = token.Claims
            .FirstOrDefault(
                x => x.Type == ClaimTypes.Email);

        Assert.NotNull(nameIdentifier);
        Assert.Equal("1", nameIdentifier!.Value);

        Assert.NotNull(email);

        // Your controller currently puts ClientSecret
        // into the Email claim.
        Assert.Equal(
            ClientSecret,
            email!.Value);
    }


    // =========================================================
    // JWT Issuer
    // =========================================================

    [Fact]
    public void Login_ShouldGenerateTokenWithCorrectIssuer()
    {
        // Arrange
        var request = new LoginRequest
        {
            ClientId = ClientId,
            ClientSecret = ClientSecret
        };

        // Act
        var result = _controller.Login(request);

        var okResult = Assert.IsType<OkObjectResult>(result);

        var tokenProperty = okResult.Value!
            .GetType()
            .GetProperty("token");

        var tokenString = tokenProperty!
            .GetValue(okResult.Value)?
            .ToString();

        var handler = new JwtSecurityTokenHandler();

        var token = handler.ReadJwtToken(tokenString);

        // Assert
        Assert.Equal(
            JwtIssuer,
            token.Issuer);
    }


    // =========================================================
    // JWT Audience
    // =========================================================

    [Fact]
    public void Login_ShouldGenerateTokenWithCorrectAudience()
    {
        // Arrange
        var request = new LoginRequest
        {
            ClientId = ClientId,
            ClientSecret = ClientSecret
        };

        // Act
        var result = _controller.Login(request);

        var okResult = Assert.IsType<OkObjectResult>(result);

        var tokenProperty = okResult.Value!
            .GetType()
            .GetProperty("token");

        var tokenString = tokenProperty!
            .GetValue(okResult.Value)?
            .ToString();

        var handler = new JwtSecurityTokenHandler();

        var token = handler.ReadJwtToken(tokenString);

        // Assert
        Assert.Contains(
            JwtAudience,
            token.Audiences);
    }


    // =========================================================
    // JWT Expiration
    // =========================================================

    [Fact]
    public void Login_ShouldGenerateTokenWithFutureExpiration()
    {
        // Arrange
        var request = new LoginRequest
        {
            ClientId = ClientId,
            ClientSecret = ClientSecret
        };

        var beforeLogin = DateTime.UtcNow;

        // Act
        var result = _controller.Login(request);

        var afterLogin = DateTime.UtcNow;

        var okResult = Assert.IsType<OkObjectResult>(result);

        var tokenProperty = okResult.Value!
            .GetType()
            .GetProperty("token");

        var tokenString = tokenProperty!
            .GetValue(okResult.Value)?
            .ToString();

        var handler = new JwtSecurityTokenHandler();

        var token = handler.ReadJwtToken(tokenString);

        // Assert
        Assert.True(token.ValidTo > beforeLogin);
        Assert.True(token.ValidTo > afterLogin);

        // Approximately 60 minutes
        var expectedExpiration =
            beforeLogin.AddMinutes(60);

        Assert.InRange(
            token.ValidTo,
            expectedExpiration.AddSeconds(-5),
            expectedExpiration.AddSeconds(5));
    }


    // =========================================================
    // Missing/empty credentials
    // =========================================================

    [Fact]
    public void Login_WithEmptyClientId_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            ClientId = string.Empty,
            ClientSecret = ClientSecret
        };

        // Act
        var result = _controller.Login(request);

        // Assert
        var unauthorizedResult =
            Assert.IsType<UnauthorizedObjectResult>(result);

        Assert.Equal(401, unauthorizedResult.StatusCode);
    }


    [Fact]
    public void Login_WithEmptyClientSecret_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            ClientId = ClientId,
            ClientSecret = string.Empty
        };

        // Act
        var result = _controller.Login(request);

        // Assert
        var unauthorizedResult =
            Assert.IsType<UnauthorizedObjectResult>(result);

        Assert.Equal(401, unauthorizedResult.StatusCode);
    }
}