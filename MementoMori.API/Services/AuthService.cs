using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace MementoMori.API.Services;

public class AuthService : IAuthService
{
    public string HashPassword(string password)
    {
        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));

        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string password, string storedHash)
    {
        var hashedPassword = HashPassword(password);
        
        return hashedPassword == storedHash;
    }

    public async void AddCookie(HttpContext httpContext, Guid userId, bool isPersistent)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
        {
            IsPersistent = isPersistent,
            ExpiresUtc = DateTime.UtcNow.AddDays(10)
        });
    }

    public void RemoveCookie(HttpContext httpContext)
    {
        httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public Guid? GetRequesterId(HttpContext httpContext)
    {
        var claim = httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(claim, out Guid requesterId))
            return requesterId;

        return null;
    }
}