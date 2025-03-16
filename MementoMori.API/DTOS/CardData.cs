namespace MementoMori.API.DTOS;

public class CardData
{
    public Guid DeckId { get; set; }
    public required string Question { get; set; }
    public required string Answer { get; set; }
}