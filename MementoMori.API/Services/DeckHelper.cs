using MementoMori.API.Data;
using MementoMori.API.Models;
using MementoMori.API.Exceptions;
using MementoMori.API.Entities;
using Microsoft.EntityFrameworkCore;
using MementoMori.API.Constants;

namespace MementoMori.API.Services;

public class DeckHelper(AppDbContext context) : IDeckHelper
{
    public List<Deck> Filter(Guid[]? ids = null, string? titleSubstring = null, string[]? selectedTags = null, Guid? userId = null)
    {
        var Decks = context.Decks.Include(deck => deck.Cards).Include(deck => deck.Creator).Where(deck => deck.IsPublic || deck.CreatorId == userId);

        if (ids != null && ids.Length > 0)
            Decks = Decks.Where(deck => ids.Contains(deck.Id));

        if (!string.IsNullOrEmpty(titleSubstring))
        {
            Decks = Decks.Where(deck => EF.Functions.ILike(deck.Title, $"%{titleSubstring}%"));
        }

        if (selectedTags != null && selectedTags.Length != 0)
        {
            TagTypes[] selectedTagEnums;
            try
            {
                selectedTagEnums = [.. selectedTags.Select(tag => Enum.Parse<TagTypes>(tag))];
            }
            catch (Exception)
            {
                return [];
            }

            Decks = Decks.Where(deck => deck.Tags != null && !selectedTagEnums.Except(deck.Tags).Any());
        }

        return [.. Decks];
    }

    public async Task UpdateDeckAsync(EditedDeckDTO editedDeckDTO, Guid requesterId)
    {
        try
        {
            await context.SecureUpdateAsync<Deck, DeckEditableProperties>(editedDeckDTO.Deck, requesterId);

            if (editedDeckDTO.Cards != null)
                foreach (CardEditableProperties card in editedDeckDTO.Cards)
                    await context.SecureUpdateAsync<Card, CardEditableProperties>(card, editedDeckDTO.Deck.Id);
            
            if (editedDeckDTO.NewCards != null)
            {
                foreach (Card card in editedDeckDTO.NewCards)
                {
                    card.DeckId = editedDeckDTO.Deck.Id;
                    context.Add(card);
                }
            }

            if (editedDeckDTO.RemovedCards != null)
                foreach (Guid cardId in editedDeckDTO.RemovedCards)
                    await context.RemoveAsync<Card>(cardId, editedDeckDTO.Deck.Id);
            
            await context.SaveChangesAsync();
        }
        catch (UnauthorizedEditingException)
        {
            throw;
        }
    }
    public async Task<Guid> CreateDeckAsync(EditedDeckDTO createDeck, Guid requesterId)
    {
        var newDeckGuid = Guid.NewGuid();
        if (createDeck.Deck.Title == "" || createDeck.Deck.Title.TrimStart(' ').Length == 0)
            throw new ArgumentException("Deck title cannot be empty.");
        
        var cards = createDeck.NewCards?.ToList() ?? [];
        int cardCount = cards.Count;
        var newDeck = new Deck()
        {
            Id = newDeckGuid,
            CreatorId = requesterId,
            IsPublic = createDeck.Deck.IsPublic,
            Title = createDeck.Deck.Title,
            Description = createDeck.Deck.Description,
            Tags = createDeck.Deck.Tags,
            Rating = 0,
            RatingCount = 0,
            Modified = DateOnly.FromDateTime(DateTime.Now),
            Cards = cards,
            CardCount = cardCount,
        };
        context.Decks.Add(newDeck);
        
        await context.SaveChangesAsync();

        return newDeckGuid;
    }
    public async Task DeleteDeckAsync(Guid deckId, Guid requesterId)
    {
        var deck = context.Decks.Include(d => d.Cards).FirstOrDefault(d => d.Id == deckId);
        if (deck != null)
        {
            if (deck.CreatorId != requesterId)
                throw new UnauthorizedEditingException();

            context.Decks.Remove(deck);
            await context.SaveChangesAsync();
        }
        else
            throw new KeyNotFoundException();
    }
    public UserDeckDTO[] GetUserDecks(Guid userId)
    {
        var userDecks = context.Decks
            .Where(deck => deck.CreatorId == userId)
            .Select(deck => new UserDeckDTO
            {
                Id = deck.Id,
                Title = deck.Title
            })
            .ToArray();

        return userDecks;
    }
    public Deck GetDeckAsync(Guid deckId)
    {
        var deck = context.Decks
            .FirstOrDefault(d => d.Id == deckId) ?? throw new KeyNotFoundException($"Deck with ID {deckId} not found.");

        return deck;
    }
    public UserDeckDTO[] GetUserCollectionDecks(Guid userId)
    {
        var userCollectionDecks = context.UserCards
            .Where(userCard  => userCard.UserId == userId)
            .Join(context.Decks, userDeck => userDeck.DeckId, deck => deck.Id, (userDeck, deck) => new UserDeckDTO
                {
                    Id = userDeck.DeckId,
                    Title = deck.Title
                })
            .Distinct()
            .ToArray();

        return userCollectionDecks;
    }
    public void DeleteUserCollectionDeck(Guid deckId, Guid userId)
    {
        var userCardsToDelete = context.UserCards
            .Where(card => card.DeckId == deckId && card.UserId == userId)
            .ToList();

        context.UserCards.RemoveRange(userCardsToDelete);
        context.SaveChanges();

        return;
    }

    public bool IsDeckInCollection(Guid deckId, Guid? userId) 
    {
        if (userId == null) 
            return false;

        var decks = GetUserCollectionDecks(userId.Value);
        return decks.Any(x => x.Id == deckId);
    }
}