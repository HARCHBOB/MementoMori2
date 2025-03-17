namespace MementoMori.API.Entities;

public class Card : CardEditableProperties
{
    public Guid DeckId { get; set;  }

    public override bool CanEdit(Guid editorId)
    {
        return DeckId == editorId;
    }
}