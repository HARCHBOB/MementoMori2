namespace MementoMori.API.DTOS;

public class LoginDetails
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public bool RememberMe { get; set; } = false;
}