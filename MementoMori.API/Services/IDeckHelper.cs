using MementoMori.API.Models;
using MementoMori.API.Entities;

namespace MementoMori.API.Services;

public interface IDeckHelper
{
    List<Deck> Filter(Guid[]? ids = null, string? titleSubstring = null, string[]? selectedTags = null, Guid? userId = null);
    Task UpdateDeckAsync(EditedDeckDTO editedDeckDTO, Guid editorId);
    Task<Guid> CreateDeckAsync(EditedDeckDTO createDeck, Guid requesterId);
    Task DeleteDeckAsync(Guid deckId, Guid requesterId);
    UserDeckDTO[] GetUserDecks(Guid userId);
    UserDeckDTO[] GetUserCollectionDecks(Guid userId);
    Deck? GetDeckAsync(Guid deckId);
    void DeleteUserCollectionDeck(Guid deckId, Guid userId);
    bool IsDeckInCollection(Guid deckId, Guid? userId);
}