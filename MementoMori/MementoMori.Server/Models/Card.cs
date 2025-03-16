using MementoMori.Server.Models;

namespace MementoMori.Server;

public class Card : CardEditableProperties
{
    public Guid DeckId { get; set;  }

    public override bool CanEdit(Guid editorId)
    {
        return DeckId == editorId;
    }
}