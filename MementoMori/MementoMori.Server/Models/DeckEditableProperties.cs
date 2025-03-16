namespace MementoMori.Server.Models;

public class DeckEditableProperties : DatabaseObject
{
    public required bool isPublic { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public List<TagTypes>? Tags { get; set; }
}