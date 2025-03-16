using MementoMori.API.Entities;

namespace MementoMori.API.Models;

public class EditedDeckDTO
{
    public required DeckEditableProperties Deck { get; set; }
    public CardEditableProperties[]? Cards { get; set; }
    public Card[]? NewCards { get; set; }
    public Guid[]? RemovedCards { get; set; }
}