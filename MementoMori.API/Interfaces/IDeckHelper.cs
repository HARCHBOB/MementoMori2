using MementoMori.API.DTOS;
using MementoMori.API.Models;

namespace MementoMori.API.Interfaces;

public interface IDeckHelper
{
    Task<List<Deck>> Filter(Guid[]? ids = null, string? titleSubstring = null, string[]? selectedTags = null, Guid? userId = null);
    Task UpdateDeckAsync(EditedDeckDTO editedDeckDTO, Guid editorId);
    Task<Guid> CreateDeckAsync(EditedDeckDTO createDeck, Guid requesterId);
    Task DeleteDeckAsync(Guid deckId, Guid requesterId);
    Task<UserDeckDTO[]> GetUserDecks(Guid userId);
    Task<UserDeckDTO[]> GetUserCollectionDecks(Guid userId);
    Task<Deck> GetDeckAsync(Guid deckId);
    Task DeleteUserCollectionDeck(Guid deckId, Guid userId);
    Task<bool> IsDeckInCollection(Guid deckId, Guid? userId);
}