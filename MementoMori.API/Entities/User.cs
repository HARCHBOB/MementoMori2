namespace MementoMori.API.Entities;

public class User : DatabaseObject
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string CardColor { get; set; }

    public override bool CanEdit(Guid editorId) => Id == editorId;
}