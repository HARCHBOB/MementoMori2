namespace MementoMori.API.Models;

public class UserDeckInformationDTO
{
    public UserDeckDTO[]? Decks { get; set; }

    public bool IsLoggedIn { get; set; }

}