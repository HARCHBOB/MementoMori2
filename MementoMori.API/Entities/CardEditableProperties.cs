namespace MementoMori.API.Entities;

public class CardEditableProperties : DatabaseObject
{
    public required string Question { get; set; }

    public string? Description { get; set; }

    public required string Answer { get; set; }
}