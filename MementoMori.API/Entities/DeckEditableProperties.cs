using MementoMori.API.Constants;

namespace MementoMori.API.Entities;

public class DeckEditableProperties : DatabaseObject
{
    public required bool IsPublic { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public List<TagTypes>? Tags { get; set; }
}