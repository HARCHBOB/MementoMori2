namespace MementoMori.Server.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException() : base("This user does not exist") 
    {
    }
}