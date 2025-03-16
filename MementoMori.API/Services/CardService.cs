using MementoMori.API.Data;
using MementoMori.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace MementoMori.API.Services;

public class CardService(AppDbContext context, ISpacedRepetition spacedRepetitionService) : ICardService
{
    public void AddCardsToCollection(Guid userId, Guid deckId)
    {
        var deck = context.Decks
            .Include(d => d.Cards)
            .FirstOrDefault(d => d.Id == deckId) ?? throw new Exception("Deck not found.");

        var existingUserCards = context.UserCards
            .Where(uc => uc.UserId == userId && uc.DeckId == deckId)
            .Select(uc => uc.CardId)
            .ToHashSet();

        var newUserCards = new List<UserCardData>();

        foreach (var card in deck.Cards)
        {
            if (!existingUserCards.Contains(card.Id))
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

        if (newUserCards.Count != 0)
        {
            context.UserCards.AddRange(newUserCards);
            context.SaveChanges(); 
        }
    }

    public async Task UpdateSpacedRepetition(Guid userId, Guid deckId, Guid cardId, int quality)
    {
        var userCardData = await context.UserCards
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.DeckId == deckId && uc.CardId == cardId) ?? throw new KeyNotFoundException("Card data not found for the specified user, deck, and card.");

        spacedRepetitionService.UpdateCard(userCardData, quality);

        userCardData.LastReviewed = DateTime.UtcNow; 
        context.UserCards.Update(userCardData);

        await context.SaveChangesAsync();
    }


    public List<Card> GetCardsForReview(Guid deckId, Guid userId)
    {
        var cardsForReview = 
            context.UserCards
                .Where(uc => uc.DeckId == deckId
                            && uc.UserId == userId && uc.LastReviewed.AddDays(uc.Interval) <= DateTime.UtcNow.Date)
                .Select(uc => uc.CardId)
                .ToList();

        return [.. context.Cards.Where(c => cardsForReview.Contains(c.Id))];

    }
}