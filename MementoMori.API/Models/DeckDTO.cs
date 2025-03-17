namespace MementoMori.API.Models;

public class DeckDTO
{
    public required Guid Id { get; set; }

    public required string CreatorName { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public double Rating { get; set; }

    public required DateOnly Modified { get; set; }

    public required long CardCount { get; set; }

    public List<string>? Tags { get; set; }

    public bool IsOwner { get; set; }

    public bool InCollection { get; set; }
}