using MementoMori.API.Controllers;
using MementoMori.API.Models;
using MementoMori.API.Services;
using MementoMori.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MementoMori.API.Constants;

namespace MementoMori.Tests.UnitTests.ControllerTests;

public class DeckBrowserControllerTests
{
    private readonly Mock<IDeckHelper> _mockDeckHelper;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly DeckBrowserController _controller;

    public DeckBrowserControllerTests()
    {
        _mockDeckHelper = new Mock<IDeckHelper>();
        _mockAuthService = new Mock<IAuthService>();

        _controller = new DeckBrowserController(_mockDeckHelper.Object, _mockAuthService.Object);
    }

    [Fact]
    public void GetDecksAsync_ReturnsEmptyList_WhenNoDecksMatchFilter()
    {
        var tags = new string[] { "Science" };
        var searchString = "Physics";
        _mockDeckHelper
            .Setup(d => d.Filter(It.IsAny<Guid[]>(), searchString, tags, null))
            .Returns([]);

        var result = _controller.GetDecksAsync(tags, searchString);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var decks = Assert.IsType<DeckBrowserDTO[]>(okResult.Value);
        Assert.Empty(decks);
    }

    [Fact]
    public void GetDecksAsync_SortsDecksByRating()
    {
        var decks = new List<Deck>
        {
            new() 
            {
                Id = Guid.NewGuid(),
                Title = "Low Rated Deck",
                Rating = 3.0,
                RatingCount = 40,
                Modified = DateOnly.FromDateTime(DateTime.UtcNow),
                CardCount = 5,
                IsPublic = true,
            },
            new() 
            {
                Id = Guid.NewGuid(),
                Title = "High Rated Deck",
                Rating = 5.0,
                RatingCount = 100,
                Modified = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                CardCount = 15,
                IsPublic = true,
            },
            new() 
            {
                Id = Guid.NewGuid(),
                Title = "Medium Rated Deck",
                Rating = 4.0,
                RatingCount= 1,
                Modified = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)),
                CardCount = 10,
                Tags = [TagTypes.History],
                IsPublic = true,
            }
        };
        _mockDeckHelper
            .Setup(dh => dh.Filter(It.IsAny<Guid[]>(), It.IsAny<string>(), It.IsAny<string[]>(), null))
            .Returns(decks);

        var result = _controller.GetDecksAsync([], null);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var deckDTOs = Assert.IsType<DeckBrowserDTO[]>(okResult.Value);

        Assert.Equal(3, deckDTOs.Length);
        Assert.Equal("High Rated Deck", deckDTOs[0].Title);
        Assert.Equal("Low Rated Deck", deckDTOs[1].Title);
        Assert.Equal("Medium Rated Deck", deckDTOs[2].Title);
    }


    [Fact]
    public void GetDecksAsync_ReturnsAllDecks_WhenNoFiltersProvided()
    {
        var decks = new List<Deck>
        {
            new() 
            {
                Id = Guid.NewGuid(),
                Title = "Deck 1",
                Rating = 4.0,
                Modified = DateOnly.FromDateTime(DateTime.UtcNow),
                CardCount = 5,
                Tags = [TagTypes.History],
                IsPublic = true,
            },
            new() 
            {
                Id = Guid.NewGuid(),
                Title = "Deck 2",
                Rating = 4.2,
                Modified = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                CardCount = 10,
                Tags = [TagTypes.Music],
                IsPublic = true,
            }
        };
        _mockDeckHelper
            .Setup(dh => dh.Filter(It.IsAny<Guid[]>(), It.IsAny<string>(), It.IsAny<string[]>(), null))
            .Returns(decks);

        var result = _controller.GetDecksAsync([], null);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var deckDTOs = Assert.IsType<DeckBrowserDTO[]>(okResult.Value);
        Assert.Equal(2, deckDTOs.Length);
    }
}