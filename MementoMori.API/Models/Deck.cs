using MementoMori.API.Models;

namespace MementoMori.API;

public class Deck : DeckEditableProperties, IComparable<Deck>
{
    public User? Creator { get; set; }

    public Guid CreatorId { get; set; }

    public double Rating { get; set; }

    public long RatingCount { get; set; }

    public required DateOnly Modified { get; set; }

    public required long CardCount { get; set; }

    public List<Card> Cards { get; set; } = [];

    public int CompareTo(Deck? other)
    {
        if (other == null) return 1;
        return (Rating * RatingCount).CompareTo(other.Rating * other.RatingCount) * -1;
    }

    public override bool CanEdit(Guid editorId)
    {
        return editorId == CreatorId;
    }
}