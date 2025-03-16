namespace MementoMori.API.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException() : base("This user does not exist") 
    {
    }
}