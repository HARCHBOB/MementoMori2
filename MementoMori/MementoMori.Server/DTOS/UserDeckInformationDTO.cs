namespace MementoMori.Server.DTOS;

public class UserDeckInformationDTO
{
    public UserDeckDTO[]? Decks { get; set; }

    public bool IsLoggedIn { get; set; }

}