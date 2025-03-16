namespace MementoMori.API.Entities;

public class DatabaseObject
{
    public Guid Id { get; set; }

    virtual public bool CanEdit(Guid editorId) => false;
}