using MementoMori.Server.Models;

namespace MementoMori.Server.DTOS;

public class DeckBrowserDTO
{
    public required Guid Id { get; set; }

    public required string Title { get; set; }

    public double Rating { get; set; }

    public long RatingCount { get; set; }

    public required DateOnly Modified { get; set; }

    public required long Cards { get; set; }

    public List<string>? Tags { get; set; }
}