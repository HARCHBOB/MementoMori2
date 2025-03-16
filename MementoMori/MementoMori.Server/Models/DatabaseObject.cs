namespace MementoMori.Server.Models;

public class DatabaseObject
{
    public Guid Id { get; set; }

    virtual public bool CanEdit(Guid editorId)
    {
        return false;
    }
}