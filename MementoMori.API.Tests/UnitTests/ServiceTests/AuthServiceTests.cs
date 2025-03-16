using MementoMori.API.Data;
using MementoMori.API.Services;
using MementoMori.API.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.AspNetCore.Http;
using MementoMori.API.Models;
using MementoMori.API.Exceptions;
using System.Security.Claims;

namespace MementoMori.API.Tests.UnitTests.ServiceTests;

public class AuthServiceTests
{
    private readonly AuthService _authService;
    private readonly AuthRepo _authRepo;
    private readonly AppDbContext _context;
    private readonly Mock<HttpContext> _mockHttpContext;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new AppDbContext(options);
        _authService = new AuthService();
        _authRepo = new AuthRepo(_context, _authService);

        _mockHttpContext = new Mock<HttpContext>();
    }

    [Fact]
    public void HashPassword_ReturnsHashedPassword()
    {
        string password = "SecurePassword123";

        string hashedPassword = _authService.HashPassword(password);

        Assert.NotNull(hashedPassword);
        Assert.NotEmpty(hashedPassword);
        Assert.NotEqual(password, hashedPassword);
    }

    [Theory]
    [InlineData("SecurePassword123!", "DifferentPassword")]
    [InlineData("SecurePassword123", "SecurePassword123")]
    public void VerifyPassword_ReturnsExpectedResult(string storedPassword, string passwordToVerify)
    {
        var storedHash = _authService.HashPassword(storedPassword);

        var result = _authService.VerifyPassword(passwordToVerify, storedHash);

        if (storedPassword == passwordToVerify)
        {
            Assert.True(result);
        }
        else
        {
            Assert.False(result);
        }
    }

    [Fact]
    public async Task CreateUserAsync_CreatesUserSuccessfully()
    {
        var registerDetails = new RegisterDetails { Username = "newuser", Password = "SecurePassword123", RememberMe = true };
        var hashedPassword = _authService.HashPassword(registerDetails.Password);

        var user = await _authRepo.CreateUserAsync(registerDetails);

        Assert.NotNull(user);
        Assert.Equal(registerDetails.Username, user.Username);
        Assert.Equal(hashedPassword, user.Password);
    }

    [Fact]
    public async Task CreateUserAsync_ThrowsException_WhenUsernameExists()
    {
        var existingUser = new User { Username = "existinguser", Password = "SecurePassword123", CardColor = "white" };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var registerDetails = new RegisterDetails { Username = "existinguser", Password = "SecurePassword123" };

        await Assert.ThrowsAsync<Exception>(async () => await _authRepo.CreateUserAsync(registerDetails));
    }

    [Fact]
    public async Task CreateUserAsync_ThrowsException_WhenPasswordIsNullOrEmpty()
    {
        var registerDetails = new RegisterDetails { Username = "somenewuser", Password = "" };

        await Assert.ThrowsAsync<Exception>(async () => await _authRepo.CreateUserAsync(registerDetails));
    }

    [Fact]
    public async Task GetUserById_ReturnsUser_WhenUserExists()
    {
        var userId = Guid.NewGuid();
        var username1 = "existinguser1";
        var username2 = "existinguser2";
        var username3 = "existinguser3";
        var password = "password";
        var passwordHash = _authService.HashPassword(password);
        var user1 = new User { Id = Guid.NewGuid(), Username = username1, Password = password, CardColor = "white" };
        var user2 = new User { Id = userId, Username = username2, Password = password, CardColor = "white" };
        var user3 = new User { Id = Guid.NewGuid(), Username = username3, Password = password, CardColor = "white" };
        _context.Users.AddRange([user1, user2]);
        await _context.SaveChangesAsync();

        var result = await _authRepo.GetUserByIdAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal(username2, result.Username);
    }

    [Fact]
    public async Task GetUserById_ThrowsUserNotFoundException_WhenUserDoesNotExist()
    {
        var userId = Guid.NewGuid();

        await Assert.ThrowsAsync<UserNotFoundException>(async () => await _authRepo.GetUserByIdAsync(userId));
    }

    [Fact]
    public async Task GetUserByUsername_ReturnsUser_WhenUserExists()
    {
        var username = "existinguser";
        var password = "password";
        var user = new User { Username = username, Password = password, CardColor = "white" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = await _authRepo.GetUserByUsernameAsync("existinguser");

        Assert.NotNull(result);
        Assert.Equal("existinguser", result.Username);
    }

    [Fact]
    public async Task GetUserByUsername_Throws_WhenUserDoesNotExist()
    {
        await Assert.ThrowsAsync<UserNotFoundException>(() =>
        {
            return _authRepo.GetUserByUsernameAsync("nonexistentuser");
        });
    }

    [Fact]
    public void GetRequesterId_ReturnsRequesterId_WhenClaimExists()
    {
        var userId = Guid.NewGuid();
        _mockHttpContext.Setup(ctx => ctx.User.Claims).Returns(new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString())
        });

        var result = _authService.GetRequesterId(_mockHttpContext.Object);

        Assert.NotNull(result);
        Assert.Equal(userId, result);
    }

    [Fact]
    public void GetRequesterId_ReturnsNull_WhenClaimDoesNotExist()
    {
        _mockHttpContext.Setup(ctx => ctx.User.Claims).Returns(new List<Claim>());

        var result = _authService.GetRequesterId(_mockHttpContext.Object);

        Assert.Null(result);
    }
}
