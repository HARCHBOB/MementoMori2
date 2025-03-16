

namespace MementoMori.Server;

public interface ICardService
{
    Task UpdateSpacedRepetition(Guid userId, Guid deckId, Guid cardId, int quality);
    void AddCardsToCollection(Guid userId, Guid deckId);
    List<Card> GetCardsForReview(Guid deckId, Guid userId);
}