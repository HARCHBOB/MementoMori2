namespace MementoMori.API.DTOS;

public class UserDeckInformationDTO
{
    public UserDeckDTO[]? Decks { get; set; }

    public bool IsLoggedIn { get; set; }

}