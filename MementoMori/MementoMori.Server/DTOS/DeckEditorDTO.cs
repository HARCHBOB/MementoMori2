using MementoMori.Server.Models;

namespace MementoMori.Server.DTOS;

public class DeckEditorDTO
{
    public required Guid Id { get; set; }

    public required bool isPublic { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public required long CardCount { get; set; }

    public List<string>? Tags { get; set; }

    public CardDTO[]? Cards { get; set; }
}