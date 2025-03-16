using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MementoMori.API.Controllers;
using MementoMori.API.Interfaces;
using MementoMori.API.DTOS;
using MementoMori.API.Models;

public class UserDecksControllerTests
{    
    private readonly Mock<IDeckHelper> _mockDeckHelper;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly UserDecksController _controller;

    public UserDecksControllerTests()
    {
        _mockDeckHelper = new Mock<IDeckHelper>();
        _mockAuthService = new Mock<IAuthService>();

        _controller = new UserDecksController(_mockDeckHelper.Object, _mockAuthService.Object);

        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }


    [Fact]
    public async Task UserCollectionDecksController_ReturnsUserDecks_WhenRequesterIdIsValid()
    {
        var requesterId = Guid.NewGuid();
        var expectedDecks = new[]
        {
            new UserDeckDTO { Id = Guid.NewGuid(), Title = "Deck 1" },
            new UserDeckDTO { Id = Guid.NewGuid(), Title = "Deck 2" }
        };
        _mockAuthService
            .Setup(s => s.GetRequesterId(It.IsAny<HttpContext>()))
            .Returns(requesterId);
        _mockDeckHelper
            .Setup(d => d.GetUserCollectionDecks(requesterId))
            .ReturnsAsync(expectedDecks);

        var result = await _controller.UserCollectionDecksController();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var userInfo = Assert.IsType<UserDeckInformationDTO>(okResult.Value);

        Assert.True(userInfo.IsLoggedIn);
        Assert.Equal(expectedDecks.Length, userInfo?.Decks?.Length);
        Assert.Equal(expectedDecks, userInfo?.Decks);
    }

    [Fact]
    public async Task UserCollectionDecksController_ReturnsLoggedOutUser_WhenRequesterIdIsNull()
    {
        _mockAuthService
            .Setup(s => s.GetRequesterId(It.IsAny<HttpContext>()))
            .Returns((Guid?)null);
        var result = await _controller.UserCollectionDecksController();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var userInfo = Assert.IsType<UserDeckInformationDTO>(okResult.Value);
        Assert.False(userInfo.IsLoggedIn);
        Assert.Null(userInfo.Decks);
    }

    [Fact]
    public async Task UserCollectionRemoveDeckController_ReturnsBadRequest_WhenDeckIdIsEmpty()
    {
        var invalidDeckId = new DatabaseObject { Id = Guid.Empty };
        var result = await _controller.UserCollectionRemoveDeckController(invalidDeckId);
        var actionResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(400, actionResult.StatusCode);
    }

    [Fact]
    public async Task UserCollectionRemoveDeckController_ReturnsUnauthorized_WhenRequesterIdIsNull()
    {
        var validDeckId = new DatabaseObject { Id = Guid.NewGuid() };
        _mockAuthService
            .Setup(s => s.GetRequesterId(It.IsAny<HttpContext>()))
            .Returns((Guid?)null);
        var result = await _controller.UserCollectionRemoveDeckController(validDeckId);
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task UserCollectionRemoveDeckController_CallsDeleteUserCollectionDeck_WhenRequesterIdIsValid()
    {
        var validDeckId = new DatabaseObject { Id = Guid.NewGuid() };
        var requesterId = Guid.NewGuid();
        _mockAuthService
            .Setup(s => s.GetRequesterId(It.IsAny<HttpContext>()))
            .Returns(requesterId);
        var result = await _controller.UserCollectionRemoveDeckController(validDeckId);
        _mockDeckHelper.Verify(d => d.DeleteUserCollectionDeck(validDeckId.Id, requesterId), Times.Once);
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UserCollectionRemoveDeckController_ReturnsOk_WhenDeckIsDeletedSuccessfully()
    {
        var validDeckId = new DatabaseObject { Id = Guid.NewGuid() };
        var requesterId = Guid.NewGuid();

        _mockAuthService
            .Setup(s => s.GetRequesterId(It.IsAny<HttpContext>()))
            .Returns(requesterId);
        _mockDeckHelper
            .Setup(d => d.DeleteUserCollectionDeck(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);
        var result = await _controller.UserCollectionRemoveDeckController(validDeckId);
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UserInformation_ReturnsUserDecks_WhenUserIsLoggedIn()
    {
        var requesterId = Guid.NewGuid();
        var userDecks = new[]
        {
            new UserDeckDTO { Id = Guid.NewGuid(), Title = "Deck 1" },
            new UserDeckDTO { Id = Guid.NewGuid(), Title = "Deck 2" }
        };

        _mockAuthService.Setup(auth => auth.GetRequesterId(It.IsAny<HttpContext>()))
            .Returns(requesterId);
        _mockDeckHelper.Setup(helper => helper.GetUserDecks(requesterId))
            .ReturnsAsync(userDecks);

        var result = await _controller.UserInformation();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var userInfo = Assert.IsType<UserDeckInformationDTO>(okResult.Value);

        Assert.True(userInfo.IsLoggedIn);
        Assert.Equal(userDecks.Length, userInfo.Decks?.Length);
        Assert.Equal(userDecks[0].Title, userInfo.Decks?[0].Title);
        Assert.Equal(userDecks[1].Title, userInfo.Decks?[1].Title);
    }

    [Fact]
    public async Task UserInformation_ReturnsEmptyDecks_WhenUserIsNotLoggedIn()
    {
        _mockAuthService.Setup(auth => auth.GetRequesterId(It.IsAny<HttpContext>()))
            .Returns((Guid?)null);

        var result = await _controller.UserInformation();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var userInfo = Assert.IsType<UserDeckInformationDTO>(okResult.Value);
        Assert.False(userInfo.IsLoggedIn);
        Assert.Null(userInfo.Decks);
    }

    [Fact]
    public async Task UserInformation_ReturnsEmptyDecks_WhenUserHasNoDecks()
    {
        var requesterId = Guid.NewGuid();

        _mockAuthService.Setup(auth => auth.GetRequesterId(It.IsAny<HttpContext>()))
            .Returns(requesterId);
        _mockDeckHelper.Setup(helper => helper.GetUserDecks(requesterId))
            .ReturnsAsync(Array.Empty<UserDeckDTO>());

        var result = await _controller.UserInformation();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var userInfo = Assert.IsType<UserDeckInformationDTO>(okResult.Value);

        Assert.True(userInfo.IsLoggedIn);
        Assert.NotNull(userInfo.Decks);
        Assert.Empty(userInfo.Decks);
    }
}