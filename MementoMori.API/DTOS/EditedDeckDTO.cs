using MementoMori.API.Models;

namespace MementoMori.API.DTOS;

public class EditedDeckDTO
{
    public required DeckEditableProperties Deck { get; set; }
    public CardEditableProperties[]? Cards { get; set; }
    public Card[]? NewCards { get; set; }
    public Guid[]? RemovedCards { get; set; }
}