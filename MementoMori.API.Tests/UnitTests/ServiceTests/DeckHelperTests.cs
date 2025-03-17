using MementoMori.API.Data;
using MementoMori.API.Models;
using MementoMori.API.Exceptions;
using MementoMori.API.Entities;
using MementoMori.API.Services;
using Microsoft.EntityFrameworkCore;
using MementoMori.API.Constants;

namespace MementoMori.API.Tests.UnitTests.ServiceTests;

public class DeckHelperTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    public static EditedDeckDTO CreateTestDeck1()
    {
        var deck = new DeckEditableProperties{
                IsPublic = true,
                Title = "Test deck",
                Description = null,
                Tags = new List<TagTypes> ([TagTypes.Beginner, TagTypes.Biology]),     
            };
        Card[] cards =
        [
            new Card
            {
                DeckId = Guid.Empty,
                Question = "What is the capital of France?",
                Description = "Geography question",
                Answer = "Paris",
            },
            new Card
            {
                DeckId = Guid.Empty,
                Question = "What is 2 + 2?",
                Description = "Simple math question",
                Answer = "4",
            },
            new Card
            {
                DeckId = Guid.Empty,
                Question = "Who wrote 'To Kill a Mockingbird'?",
                Description = "Literature question",
                Answer = "Harper Lee",
            }
        ];
        var editedDeck = new EditedDeckDTO{
            Deck = deck,
            Cards = null,
            NewCards = cards,
            RemovedCards = null,
        };
        return editedDeck;
    }
    public Deck createTestDeck2()
    {
        var deckId = Guid.NewGuid();
        var deck = new Deck
        {
            Id = deckId,
            CreatorId = Guid.NewGuid(),
            IsPublic = true,
            Title = " Test Deck",
            Description = "This is a test deck.",
            Tags = new List<TagTypes> { TagTypes.Biology },
            Modified = DateOnly.FromDateTime(DateTime.Now),
            CardCount = 2,
            Cards = new List<Card>
            {
                new() {
                    Id = Guid.NewGuid(),
                    DeckId = deckId,
                    Question = "What is the capital of France?",
                    Answer = "Paris",
                    Description = "Geography question",
                },
                new() {
                    Id = Guid.NewGuid(),
                    DeckId = deckId,
                    Question = "What is 2 + 2?",
                    Answer = "4",
                    Description = "Math question",
                }
            }
        };
        return deck;
    }

    [Fact]
    public void Filter_ReturnsEmptyList_WhenNoMatches()
    {
        var context = CreateDbContext();
        var helper = new DeckHelper(context);
        var deck1 = new Deck { Id = Guid.NewGuid(), Title = "Deck1", IsPublic = true, CardCount = 1, Modified = DateOnly.MaxValue, };
        context.Decks.Add(deck1);
        context.SaveChanges();

        var result = helper.Filter(selectedTags: ["Mathematics"]);

        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateDeck_UpdatesDeckDetails()
    {
        var context = CreateDbContext();
        var helper = new DeckHelper(context);
        var creatorId = Guid.NewGuid();
        var deck = new Deck { Id = Guid.NewGuid(), Title = "Old Title", IsPublic = true, CardCount = 1, Modified = DateOnly.MaxValue, CreatorId = creatorId };
        context.Decks.Add(deck);
        context.SaveChanges();
        var updatedDeckDTO = new EditedDeckDTO
        {
            Deck = new DeckEditableProperties
            {
                Id = deck.Id,
                Title = "New Title",
                IsPublic = false
            }
        };

        await helper.UpdateDeckAsync(updatedDeckDTO, creatorId);

        var updatedDeck = context.Decks.First();
        Assert.Equal("New Title", updatedDeck.Title);
        Assert.False(updatedDeck.IsPublic);
    }

    [Fact]
    public async Task UpdateDeck_AddsNewCards()
    {
        var context = CreateDbContext();
        var helper = new DeckHelper(context);
        var creatorId = Guid.NewGuid();
        var deck = new Deck { Id = Guid.NewGuid(), Title = "Deck1", IsPublic = true, CardCount = 1, Modified = DateOnly.MaxValue, CreatorId = creatorId };
        context.Decks.Add(deck);
        context.SaveChanges();
        var newCard = new Card { Id = Guid.NewGuid(), Question = "New Question", Answer = "New Answer" };
        var updatedDeckDTO = new EditedDeckDTO
        {
            Deck = new DeckEditableProperties { Id = deck.Id, IsPublic = true, Title = "test" },
            NewCards = [newCard]
        };

        await helper.UpdateDeckAsync(updatedDeckDTO, creatorId);

        var addedCard = context.Cards.First();
        Assert.Equal(newCard.Question, addedCard.Question);
        Assert.Equal(deck.Id, addedCard.DeckId);
    }

    [Fact]
    public async void UpdateDeck_RemovesCards()
    {
        var context = CreateDbContext();
        var helper = new DeckHelper(context);
        var creatorId = Guid.NewGuid();
        var deck = new Deck { Id = Guid.NewGuid(), Title = "Deck1", IsPublic = true, CardCount = 1, Modified = DateOnly.MaxValue, CreatorId = creatorId };
        var card = new Card { Id = Guid.NewGuid(), Question = "Question1", Answer = "Answer1", DeckId = deck.Id };
        context.Decks.Add(deck);
        context.Cards.Add(card);
        context.SaveChanges();
        var updatedDeckDTO = new EditedDeckDTO
        {
            Deck = new DeckEditableProperties { Id = deck.Id, IsPublic = true, Title = "Title" },
            RemovedCards = [card.Id]
        };

        await helper.UpdateDeckAsync(updatedDeckDTO, creatorId);

        Assert.Empty(context.Cards);
    }

    [Fact]
    public async Task CreateDeck_CreatesNewDeckSuccessfully()
    {
        var context = CreateDbContext();
        var helper = new DeckHelper(context);

        var deck = CreateTestDeck1();

        Guid requesterId = Guid.NewGuid();

        Guid savedDeckId = await helper.CreateDeckAsync(deck, requesterId);

        var savedDeck = context.Decks.Include(d => d.Cards).FirstOrDefault(d => d.Id == savedDeckId);

        Assert.NotNull(savedDeck);
        Assert.Equal(requesterId, savedDeck.CreatorId);
        Assert.Equal(deck.Deck.Title, savedDeck.Title);
        Assert.Null(savedDeck.Description);
        Assert.True(savedDeck.IsPublic);
        Assert.Equal(deck.Deck.Tags, savedDeck.Tags);
        Assert.NotNull(savedDeck.Cards);
        Assert.Equal(deck.NewCards?.Length, savedDeck.Cards.Count);
    }

    [Fact]
    public async Task CreateDeck_FailsWithArgumentException()
    {
        var context = CreateDbContext();
        var helper = new DeckHelper(context);

        var invalidDeckDTO = new EditedDeckDTO
        {
            Deck = new DeckEditableProperties
            {
                IsPublic = true,
                Title = "   ",
                Description = null,
                Tags = new List<TagTypes> { TagTypes.Biology }
            },
            NewCards =
            [
                new Card { Question = "Question?", Answer = "Answer", DeckId = Guid.Empty }
            ]
        };
        Guid requesterId = Guid.NewGuid();
        await Assert.ThrowsAsync<ArgumentException>(() => helper.CreateDeckAsync(invalidDeckDTO, requesterId));
    }
    [Fact]
    public async Task DeleteDeck_DeleteDeckSuccessfully()
    {
        var context = CreateDbContext();
        var helper = new DeckHelper(context);

        var deck = createTestDeck2();
        Guid requesterId = deck.CreatorId;
        context.Decks.Add(deck);
        await context.SaveChangesAsync();
        await helper.DeleteDeckAsync(deck.Id, requesterId);

        var deletedDeck = await context.Decks.FindAsync(deck.Id);
        Assert.Null(deletedDeck);

        var associatedCards = context.Cards.Where(c => c.DeckId == deck.Id).ToList();
        Assert.Empty(associatedCards);
                
    }

    [Fact]
    public async Task DeleteDeck_ThrowsUnauthorizedEditingException_WhenUserIsNotCreator()
    {
        var context = CreateDbContext();
        var helper = new DeckHelper(context);

        var deck = createTestDeck2();
        Guid requesterId = Guid.NewGuid();
        context.Decks.Add(deck);
        await context.SaveChangesAsync();
        var exception = await Assert.ThrowsAsync<UnauthorizedEditingException>(() => helper.DeleteDeckAsync(deck.Id, requesterId));
        Assert.NotNull(exception); 
    }

    [Fact]
    public async Task DeleteDeck_KeyNotFoundException()
    {
        var context = CreateDbContext();
        var helper = new DeckHelper(context);

        Guid nonExistentDeckId = Guid.NewGuid();
        Guid requesterId = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => helper.DeleteDeckAsync(nonExistentDeckId, requesterId));
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task GetUserDecks_ReturnsUserDecks_WhenUserHasDecks()
    {
        var context = CreateDbContext();
        var helper = new DeckHelper(context);
        var userId = Guid.NewGuid();

        var decks = new List<Deck>
        {
            new() { 
                Id = Guid.NewGuid(), 
                CreatorId = userId, 
                Title = "Deck 1", 
                IsPublic = true,
                Modified = DateOnly.FromDateTime(DateTime.Now),
                CardCount = 0
            },
            new() { 
                Id = Guid.NewGuid(), 
                CreatorId = userId, 
                Title = "Deck 2", 
                IsPublic = false,
                Modified = DateOnly.FromDateTime(DateTime.Now),
                CardCount = 0
            }
        };

        context.Decks.AddRange(decks);
        await context.SaveChangesAsync();
        var userDecks = helper.GetUserDecks(userId);

        Assert.NotNull(userDecks);
        Assert.Equal(2, userDecks.Length);
        Assert.Contains(userDecks, d => d.Title == "Deck 1");
        Assert.Contains(userDecks, d => d.Title == "Deck 2");
    }

    [Fact]
    public async Task GetUserDecks_ReturnsEmptyArray_WhenUserHasNoDecks()
    {
        var context = CreateDbContext();
        var helper = new DeckHelper(context);
        var userId = Guid.NewGuid();
        await context.SaveChangesAsync();
        var userDecks = helper.GetUserDecks(userId);

        Assert.NotNull(userDecks);
        Assert.Empty(userDecks);
    }

    [Fact]
    public async Task GetUserDecks_ReturnsOnlyUserOwnedDecks()
    {
        var context = CreateDbContext();
        var helper = new DeckHelper(context);
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var decks = new List<Deck>
        {
            new() { 
                Id = Guid.NewGuid(), 
                CreatorId = userId, 
                Title = "User's Deck", 
                IsPublic = false,
                Modified = DateOnly.FromDateTime(DateTime.Now),
                CardCount = 0
            },
            new() { 
                Id = Guid.NewGuid(), 
                CreatorId = otherUserId, 
                Title = "Other User's Deck", 
                IsPublic = true,
                Modified = DateOnly.FromDateTime(DateTime.Now),
                CardCount = 0
            }
        };

        context.Decks.AddRange(decks);
        await context.SaveChangesAsync();
        var userDecks = helper.GetUserDecks(userId);

        Assert.NotNull(userDecks);
        Assert.Single(userDecks);
        Assert.Equal("User's Deck", userDecks[0].Title);
    }

    [Fact]
    public async Task GetUserDecks_ReturnsCorrectDTOProperties()
    {
        var context = CreateDbContext();
        var helper = new DeckHelper(context);
        var userId = Guid.NewGuid();

        var deck = new Deck 
        { 
            Id = Guid.NewGuid(), 
            CreatorId = userId, 
            Title = "Test Deck", 
            IsPublic = true,
            Modified = DateOnly.FromDateTime(DateTime.Now),
            CardCount = 0
        };

        context.Decks.Add(deck);
        await context.SaveChangesAsync();
        var userDecks = helper.GetUserDecks(userId);

        Assert.NotNull(userDecks);
        Assert.Single(userDecks);
        
        var userDeck = userDecks[0];

        Assert.Equal(deck.Id, userDeck.Id);
        Assert.Equal(deck.Title, userDeck.Title);
    }

    [Fact]
    public async Task GetUserCollectionDecks_ReturnsCorrectDecks()
    {
        var context = CreateDbContext();
        var helper = new DeckHelper(context);

        var userId = Guid.NewGuid();
        var deck1Id = Guid.NewGuid();
        var deck2Id = Guid.NewGuid();
        var deck1 = new Deck { Id = deck1Id, Title = "Deck 1", CreatorId = userId, IsPublic = true, CardCount = 1, Modified = DateOnly.FromDateTime(DateTime.UtcNow) };
        var deck2 = new Deck { Id = deck2Id, Title = "Deck 2", CreatorId = userId, IsPublic = false, CardCount = 1, Modified = DateOnly.FromDateTime(DateTime.UtcNow) };
        var userCard1 = new UserCardData { UserId = userId, DeckId = deck1Id, CardId = Guid.NewGuid() };
        var userCard2 = new UserCardData { UserId = userId, DeckId = deck2Id, CardId = Guid.NewGuid() };

        await context.Decks.AddRangeAsync(deck1, deck2);
        await context.UserCards.AddRangeAsync(userCard1, userCard2);
        await context.SaveChangesAsync();

        var result = helper.GetUserCollectionDecks(userId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Length);
        Assert.Contains(result, d => d.Id == deck1Id && d.Title == "Deck 1");
        Assert.Contains(result, d => d.Id == deck2Id && d.Title == "Deck 2");
    }

    [Fact]
    public void GetUserCollectionDecks_ReturnsEmptyArray_WhenNoDecksFound()
    {
        var context = CreateDbContext();
        var helper = new DeckHelper(context);
        var userId = Guid.NewGuid();
        var result = helper.GetUserCollectionDecks(userId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task DeleteUserCollectionDeck_RemovesCardsCorrectly()
    {
        var context = CreateDbContext();
        var helper = new DeckHelper(context);
        var userId = Guid.NewGuid();
        var deckId = Guid.NewGuid();
        var userCard1 = new UserCardData { UserId = userId, DeckId = deckId, CardId = Guid.NewGuid() };
        var userCard2 = new UserCardData { UserId = userId, DeckId = deckId, CardId = Guid.NewGuid() };

        await context.UserCards.AddRangeAsync(userCard1, userCard2);
        await context.SaveChangesAsync();
        
        helper.DeleteUserCollectionDeck(deckId, userId);
        var remainingCards = context.UserCards.Where(card => card.DeckId == deckId && card.UserId == userId).ToList();
        Assert.Empty(remainingCards);
    }

    [Fact]
    public async Task UpdateDeckAsync_UpdatesCards_Successfully()
    {
        var context = CreateDbContext();
        var helper = new DeckHelper(context);
        var deck = createTestDeck2();
        context.Decks.Add(deck);
        await context.SaveChangesAsync();
        var editor = deck.CreatorId;
        var editedDeck = new EditedDeckDTO{
            Deck = deck,
            Cards =
            [
                new CardEditableProperties {Id = deck.Cards[0].Id, Question = "New Card 1 Question", Answer = deck.Cards[0].Answer,  Description = deck.Cards[0].Description},
                new CardEditableProperties {Id = deck.Cards[1].Id, Question = deck.Cards[1].Question, Answer = "New Card 2 Answer",  Description = "New Card 2 Description"},                
            ]
        };
        await helper.UpdateDeckAsync(editedDeck, editor);
        var updatedDeck = await context.Decks.Include(d => d.Cards).FirstOrDefaultAsync(d => d.Id == deck.Id);
        Assert.NotNull(updatedDeck);
        Assert.Equal(deck.Id, updatedDeck.Id);   
        Assert.Equal("New Card 1 Question", updatedDeck.Cards[0].Question);
        Assert.Equal(deck.Cards[0].Answer, updatedDeck.Cards[0].Answer);
        Assert.Equal(deck.Cards[0].Description, updatedDeck.Cards[0].Description);
        Assert.Equal(deck.Cards[1].Question, updatedDeck.Cards[1].Question);
        Assert.Equal("New Card 2 Answer", updatedDeck.Cards[1].Answer);
        Assert.Equal("New Card 2 Description", updatedDeck.Cards[1].Description);
    }
    [Fact]
    public async Task UpdateDeckAsync_UpdatesDeck_Successfully()
    {
        var context = CreateDbContext();
        var helper = new DeckHelper(context);
        var deck = createTestDeck2();
        
        context.Decks.Add(deck);
        await context.SaveChangesAsync();
        
        var editor = deck.CreatorId;
        var newTags = new List<TagTypes> { TagTypes.Advanced, TagTypes.Art };
        var editedDeck = new EditedDeckDTO{
            Deck = new DeckEditableProperties{
                Id = deck.Id,
                IsPublic = false,
                Title = "New Title",
                Description = "New Description",
                Tags = newTags,
            }
        };
        
        await helper.UpdateDeckAsync(editedDeck, editor);
        var updatedDeck = await context.Decks.Include(d => d.Cards).FirstOrDefaultAsync(d => d.Id == deck.Id);
        Assert.NotNull(updatedDeck);
        Assert.False(updatedDeck.IsPublic);
        Assert.Equal("New Title", updatedDeck.Title);
        Assert.Equal("New Description", updatedDeck.Description);
        Assert.Equal(newTags, updatedDeck.Tags);
    }
    [Fact]
    public async Task UpdateDeckAsync_AddNewCards_Successfully()
    {
        var context = CreateDbContext();
        var helper = new DeckHelper(context);
        var deckId = Guid.NewGuid();
        var editor = Guid.NewGuid();
        var originalDeck = new Deck{
            Id = deckId,
            CreatorId = editor,
            IsPublic = true,
            Title = "Test Deck",
            Description = "Test Description",
            CardCount = 0,
            Rating = 0,
            RatingCount = 0,
            Modified = DateOnly.FromDateTime(DateTime.Now),
            Tags = new List<TagTypes> { TagTypes.Music, TagTypes.Mathematics },

        };
        context.Decks.Add(originalDeck);
        context.SaveChanges();
        var deck = new EditedDeckDTO
        {
            Deck = originalDeck,
            NewCards = [
                new Card{
                DeckId = deckId,
                Question = "Q1",
                Answer = "A1"}, 
                new Card{
                DeckId = deckId,
                Question = "Q2",
                Answer = "A2"} 
            ]
        };
        await helper.UpdateDeckAsync(deck, editor);
        var updatedDeck = await context.Decks.Include(d => d.Cards).FirstOrDefaultAsync(d => d.Id == deckId);
        Assert.NotNull(updatedDeck);
        Assert.Equal("Q1", updatedDeck.Cards[0].Question);
        Assert.Equal("A1", updatedDeck.Cards[0].Answer);
        Assert.Equal("Q2", updatedDeck.Cards[1].Question);
        Assert.Equal("A2", updatedDeck.Cards[1].Answer);
    }

    [Fact]
    public async Task UpdateDeckAsync_RemovesCards_Successfully()
    {
        var context = CreateDbContext();
        var helper = new DeckHelper(context);
        var deck = createTestDeck2();
        context.Decks.Add(deck);
        await context.SaveChangesAsync();
        var editor = deck.CreatorId;
        var editedDeck = new EditedDeckDTO
        {
            Deck = new DeckEditableProperties
            {
                Id = deck.Id,
                IsPublic = deck.IsPublic,
                Title = deck.Title,
                Description = deck.Description,
                Tags = deck.Tags
            },
            RemovedCards = [deck.Cards[0].Id, deck.Cards[1].Id]
        };

        await helper.UpdateDeckAsync(editedDeck, editor);
        var updatedDeck = await context.Decks.Include(d => d.Cards).FirstOrDefaultAsync(d => d.Id == deck.Id);
        Assert.NotNull(updatedDeck);
        Assert.Empty(updatedDeck.Cards);
        Assert.DoesNotContain(updatedDeck.Cards, c => c.Id == deck.Cards[0].Id);
        Assert.DoesNotContain(updatedDeck.Cards, c => c.Id == deck.Cards[1].Id);
    }

    [Fact]
    public async Task UpdateDeckAsync_ThrowsUnauthorizedEditingException_ForInvalidEditor()
    {
        var context = CreateDbContext();
        var helper = new DeckHelper(context);
        var deck = createTestDeck2();
        context.Decks.Add(deck);
        await context.SaveChangesAsync();
        var invalidEditor = Guid.NewGuid();
        var editedDeck = new EditedDeckDTO
        {
            Deck = new DeckEditableProperties
            {
                Id = deck.Id,
                IsPublic = deck.IsPublic,
                Title = "Unauthorized Update",
                Description = deck.Description,
                Tags = deck.Tags
            }
        };

        await Assert.ThrowsAsync<UnauthorizedEditingException>(async () =>
            await helper.UpdateDeckAsync(editedDeck, invalidEditor));
    }

}