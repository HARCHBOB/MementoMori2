using MementoMori.API.Models;

namespace MementoMori.API;

public class Card : CardEditableProperties
{
    public Guid DeckId { get; set;  }

    public override bool CanEdit(Guid editorId)
    {
        return DeckId == editorId;
    }
}