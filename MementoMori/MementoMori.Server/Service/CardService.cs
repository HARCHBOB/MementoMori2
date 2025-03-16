using MementoMori.Server.Database;
using MementoMori.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace MementoMori.Server.Service;

public class CardService : ICardService
{
    private readonly AppDbContext _context;
    private readonly ISpacedRepetition _spacedRepetitionService;
    public CardService(AppDbContext context, ISpacedRepetition spacedRepetitionService)
    {
        _context = context;
        _spacedRepetitionService = spacedRepetitionService;
    }
    public void AddCardsToCollection(Guid userId, Guid deckId)
    {
        var deck = _context.Decks
            .Include(d => d.Cards)
            .FirstOrDefault(d => d.Id == deckId);

        if (deck == null)
        {
            throw new Exception("Deck not found.");
        }

        var existingUserCards = _context.UserCards
            .Where(uc => uc.UserId == userId && uc.DeckId == deckId)
            .Select(uc => uc.CardId)
            .ToHashSet();

        var newUserCards = new List<UserCardData>();

        foreach (var card in deck.Cards)
        {
            if (!existingUserCards.Contains(card.Id))
            {
                newUserCards.Add(new UserCardData
                {
                    UserId = userId,
                    DeckId = deckId,
                    CardId = card.Id,
                    Interval = 1,
                    Repetitions = 0,
                    EaseFactor = 2.5,
                    LastReviewed = DateTime.UtcNow.AddDays(-2)
                });
            }
        }

        if (newUserCards.Any())
        {
            _context.UserCards.AddRange(newUserCards);
            _context.SaveChanges(); 
        }
    }

    public async Task UpdateSpacedRepetition(Guid userId, Guid deckId, Guid cardId, int quality)
    {
        var userCardData = await _context.UserCards
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.DeckId == deckId && uc.CardId == cardId);

        if (userCardData == null)
        {
            throw new KeyNotFoundException("Card data not found for the specified user, deck, and card.");
        }

        _spacedRepetitionService.UpdateCard(userCardData, quality);

        userCardData.LastReviewed = DateTime.UtcNow; 
        _context.UserCards.Update(userCardData);

        var changes = await _context.SaveChangesAsync();
    }


    public List<Card> GetCardsForReview(Guid deckId, Guid userId)
    {
        var today = DateTime.Today;
        var cardsForReview = _context.UserCards
        .Where(uc => uc.DeckId == deckId
                    && uc.UserId == userId && uc.LastReviewed.AddDays(uc.Interval) <= today)
        .Select(uc => uc.CardId)
        .ToList();

        return _context.Cards
            .Where(c => cardsForReview.Contains(c.Id))
            .ToList();

    }
}