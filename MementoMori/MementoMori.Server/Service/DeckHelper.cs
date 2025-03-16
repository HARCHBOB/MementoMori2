using MementoMori.Server.Database;
using MementoMori.Server.DTOS;
using MementoMori.Server.Exceptions;
using MementoMori.Server.Interfaces;
using MementoMori.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace MementoMori.Server.Service;

public class DeckHelper : IDeckHelper
{
    private readonly AppDbContext _context;

    public DeckHelper(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<Deck>> Filter(Guid[]? ids = null, string? titleSubstring = null, string[]? selectedTags = null, Guid? userId = null)
    {
        var Decks = _context.Decks.Include(deck => deck.Cards).Include(deck => deck.Creator).Where(deck => deck.isPublic || deck.CreatorId == userId);

        if (ids != null && ids.Length > 0)
        {
            Decks = Decks.Where(deck => ids.Contains(deck.Id));
        }

        if (!string.IsNullOrEmpty(titleSubstring))
        {
            Decks = Decks.Where(deck => EF.Functions.ILike(deck.Title, $"%{titleSubstring}%"));
        }

        if (selectedTags != null && selectedTags.Length != 0)
        {
            TagTypes[] selectedTagEnums;
            try
            {
                selectedTagEnums = selectedTags.Select(tag => Enum.Parse<TagTypes>(tag)).ToArray();
            }
            catch (Exception)
            {
                // This state should not be reachable by using only UI
                return [];
            }

            Decks = Decks.Where(deck => deck.Tags != null && !(selectedTagEnums.Except(deck.Tags).Any()));
        }

        return (await Decks.ToListAsync());
    }

    public async Task UpdateDeckAsync(EditedDeckDTO editedDeckDTO, Guid requesterId)
    {
        try
        {
            await _context.SecureUpdateAsync<Deck, DeckEditableProperties>(editedDeckDTO.Deck, requesterId);
            if (editedDeckDTO.Cards != null)
            {
                foreach (CardEditableProperties card in editedDeckDTO.Cards)
                {
                    await _context.SecureUpdateAsync<Card, CardEditableProperties>(card, editedDeckDTO.Deck.Id);
                }
            }
            if (editedDeckDTO.NewCards != null)
            {
                foreach (Card card in editedDeckDTO.NewCards)
                {
                    card.DeckId = editedDeckDTO.Deck.Id;
                    _context.Add(card);
                }
            }
            if (editedDeckDTO.RemovedCards != null)
            {
                foreach (Guid cardId in editedDeckDTO.RemovedCards)
                {
                    await _context.RemoveAsync<Card>(cardId, editedDeckDTO.Deck.Id);
                }
            }
            await _context.SaveChangesAsync();
        }
        catch (UnauthorizedEditingException)
        {
            throw;
        }
    }
    public async Task<Guid> CreateDeckAsync(EditedDeckDTO createDeck, Guid requesterId)
    {
        Guid newDeckGuid = Guid.NewGuid();
        if (createDeck.Deck.Title == "" || createDeck.Deck.Title.TrimStart(' ').Length == 0)
        {
            throw new ArgumentException();
        }
        var cards = createDeck.NewCards?.ToList() ?? [];
        int cardCount = cards.Count;
        Deck newDeck = new()
        {
            Id = newDeckGuid,
            CreatorId = requesterId,
            isPublic = createDeck.Deck.isPublic,
            Title = createDeck.Deck.Title,
            Description = createDeck.Deck.Description,
            Tags = createDeck.Deck.Tags,
            Rating = 0,
            RatingCount = 0,
            Modified = DateOnly.FromDateTime(DateTime.Now),
            Cards = cards,
            CardCount = cardCount,
        };
        _context.Decks.Add(newDeck);
        await _context.SaveChangesAsync();
        return newDeckGuid;
    }
    public async Task DeleteDeckAsync(Guid deckId, Guid requesterId)
    {
        var deck = _context.Decks.Include(d => d.Cards).FirstOrDefault(d => d.Id == deckId);
        if (deck != null)
        {
            if (deck.CreatorId != requesterId)
            {
                throw new UnauthorizedEditingException();
            }
            _context.Decks.Remove(deck);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException();
        }
    }
    public async Task<UserDeckDTO[]> GetUserDecks(Guid userId)
    {
        var userDecks = await _context.Decks
            .Where(deck => deck.CreatorId == userId)
            .Select(deck => new UserDeckDTO
            {
                Id = deck.Id,
                Title = deck.Title
            })
            .ToArrayAsync();
        return userDecks;
    }
    public async Task<Deck> GetDeckAsync(Guid deckId)
    {
        var deck = await _context.Decks
            .FirstOrDefaultAsync(d => d.Id == deckId) ?? throw new KeyNotFoundException($"Deck with ID {deckId} not found.");
        return deck;
    }
    public async Task<UserDeckDTO[]> GetUserCollectionDecks(Guid userId)
    {
        var userCollectionDecks = await _context.UserCards
            .Where(userCard  => userCard.UserId == userId)
            .Join(_context.Decks, userDeck => userDeck.DeckId, deck => deck.Id, (userDeck, deck) => new UserDeckDTO
                {
                    Id = userDeck.DeckId,
                    Title = deck.Title
                })
            .Distinct()
            .ToArrayAsync();
        return userCollectionDecks;
    }
    public async Task DeleteUserCollectionDeck(Guid deckId, Guid userId)
    {
        var userCardsToDelete = _context.UserCards
            .Where(card => card.DeckId == deckId && card.UserId == userId)
            .ToList();
        _context.UserCards.RemoveRange(userCardsToDelete);
        await _context.SaveChangesAsync();
        return;
    }

    public async Task<Boolean> IsDeckInCollection(Guid deckId, Guid? userId) 
    {
        if (userId == null) 
        {
            return false;
        }
        var decks = await GetUserCollectionDecks((Guid)userId);
        return decks.Any(x => x.Id == deckId);
    }
}