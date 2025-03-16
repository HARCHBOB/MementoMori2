namespace MementoMori.Server.Interfaces;

public interface IAuthService
{
    void AddCookie(HttpContext httpContext, Guid userId, bool isPersistent);
    void RemoveCookie(HttpContext httpContext);
    Guid? GetRequesterId(HttpContext httpContext);
    string HashPassword(string password);
    bool VerifyPassword(string password, string storedHash);
}