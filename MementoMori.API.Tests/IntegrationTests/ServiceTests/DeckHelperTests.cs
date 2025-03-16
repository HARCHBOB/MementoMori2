using MementoMori.API.Constants;
using MementoMori.API.Data;
using MementoMori.API.Entities;
using MementoMori.API.Services;
using MementoMori.API.Tests.IntegrationTests.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace MementoMori.API.Tests.IntegrationTests.ServiceTests;

public class DeckHelperTests
{
    private readonly CustomWebAplicationFactory _application;

    public DeckHelperTests()
    {
        _application = new CustomWebAplicationFactory();

        using var scope = _application.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Create fresh database for each test
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        FillTestData(context);
    }

    [Fact]
    public void Filter_Works_WithTitleSubstring()
    {
        using var scope = _application.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var deckHelper = new DeckHelper(context);

        var filteredDecks = deckHelper.Filter(titleSubstring: "Deck 1");

        Assert.NotNull(filteredDecks);
        Assert.Single(filteredDecks);
        Assert.Equal("Test Deck 1", filteredDecks.First().Title);
    }

    [Fact]
    public void Filter_Works_WithIds()
    {
        using var scope = _application.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var deckHelper = new DeckHelper(context);
        var targetDeckId = context.Decks.First(deck => deck.Title == "Test Deck 2").Id;

        var filteredDecks = deckHelper.Filter(ids: [targetDeckId]);

        Assert.NotNull(filteredDecks);
        Assert.Single(filteredDecks);
        Assert.Equal("Test Deck 2", filteredDecks.First().Title);
    }

    [Fact]
    public void Filter_Works_WithTags()
    {
        using var scope = _application.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var deckHelper = new DeckHelper(context);

        var filteredDecks = deckHelper.Filter(selectedTags: ["Economics", "Beginner"]);

        Assert.NotNull(filteredDecks);
        Assert.Single(filteredDecks);
        Assert.Equal("Test Deck 3", filteredDecks.First().Title);
    }

    [Fact]
    public void Filter_ReturnsEmpty_WhenTagsMismatch()
    {
        using var scope = _application.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var deckHelper = new DeckHelper(context);

        var filteredDecks = deckHelper.Filter(selectedTags: ["Biology"]);

        Assert.NotNull(filteredDecks);
        Assert.Empty(filteredDecks);
    }

    [Fact]
    public void Filter_ReturnsEmpty_WhenIdsMismatch()
    {
        using var scope = _application.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var deckHelper = new DeckHelper(context);
        var nonExistentId = Guid.NewGuid();

        var filteredDecks = deckHelper.Filter(ids: [nonExistentId]);

        Assert.NotNull(filteredDecks);
        Assert.Empty(filteredDecks);
    }

    [Fact]
    public void Filter_WithMultipleFilters()
    {
        using var scope = _application.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var deckHelper = new DeckHelper(context);
        var targetDeck = context.Decks.First(deck => deck.Title == "Test Deck 2");
        var targetId = targetDeck.Id;

        var filteredDecks = deckHelper.Filter(
            ids: [targetId],
            titleSubstring: "Deck 2",
            selectedTags: ["Philosophy"]
        );

        Assert.NotNull(filteredDecks);
        Assert.Single(filteredDecks);
        Assert.Equal("Test Deck 2", filteredDecks.First().Title);
    }

    [Fact]
    public void Filter_ReturnsMultipleDecks()
    {
        // Arrange
        using var scope = _application.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var deckHelper = new DeckHelper(context);

        var filteredDecks = deckHelper.Filter(
            titleSubstring: "Test Deck",
            selectedTags: ["Economics"]);

        Assert.NotNull(filteredDecks);
        Assert.Equal(2, filteredDecks.Count);
        Assert.Contains(filteredDecks, deck => deck.Title == "Test Deck 1");
        Assert.Contains(filteredDecks, deck => deck.Title == "Test Deck 3");
    }

    [Fact]
    public void Filter_Works_WithNoFilters_ReturnsAllDecks()
    {
        using var scope = _application.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var deckHelper = new DeckHelper(context);

        var filteredDecks = deckHelper.Filter();

        Assert.NotNull(filteredDecks);
        Assert.Equal(3, filteredDecks.Count);
        Assert.Contains(filteredDecks, deck => deck.Title == "Test Deck 1");
        Assert.Contains(filteredDecks, deck => deck.Title == "Test Deck 2");
        Assert.Contains(filteredDecks, deck => deck.Title == "Test Deck 3");
    }

    private static void FillTestData(AppDbContext context)
    {
        var user = new User { Id = Guid.NewGuid(), Username = "Test User", Password = "psw", CardColor = "white" };

        var decks = new List<Deck>
        {
            new() 
            {
                Id = Guid.NewGuid(),
                Title = "Test Deck 1",
                IsPublic = true,
                Tags = [TagTypes.Economics],
                Creator = user,
                Cards = [new Card { Id = Guid.NewGuid(), Question = "Card 1", Answer = "Answer 1" }],
                CardCount = 1,
                Modified = DateOnly.MaxValue
            },
            new() 
            {
                Id = Guid.NewGuid(),
                Title = "Test Deck 2",
                IsPublic = true,
                Tags = [TagTypes.Philosophy],
                Creator = user,
                Cards = [new Card { Id = Guid.NewGuid(), Question = "Card 2", Answer = "Answer 2" }],
                CardCount = 1,
                Modified = DateOnly.MaxValue
            },
            new() 
            {
                Id = Guid.NewGuid(),
                Title = "Test Deck 3",
                IsPublic = true,
                Tags = [TagTypes.Economics, TagTypes.Beginner],
                Creator = user,
                Cards = [new Card { Id = Guid.NewGuid(), Question = "Card 3", Answer = "Answer 3" }],
                CardCount = 1,
                Modified = DateOnly.MaxValue
            }
        };

        context.Decks.AddRange(decks);
        context.SaveChanges();
    }
}