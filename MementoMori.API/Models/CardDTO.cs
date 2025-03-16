namespace MementoMori.API.Models;

public class CardDTO
{
    public required Guid Id { get; set; }

    public required string Question { get; set; }

    public string? Description { get; set; }

    public required string Answer { get; set; }
}