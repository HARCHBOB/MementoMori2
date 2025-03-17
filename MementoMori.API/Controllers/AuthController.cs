using MementoMori.API.Models;
using MementoMori.API.Exceptions;
using MementoMori.API.Services;
using MementoMori.API.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace MementoMori.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IAuthRepo _authRepo;
    private static readonly ConcurrentDictionary<string, User> _registeredUsers = new();
    private static bool initialized = false;

    public AuthController(IAuthService authService, IAuthRepo authRepo)
    {
        _authService = authService;
        _authRepo = authRepo;
        
        if (!initialized)
        {
            foreach (var user in _authRepo.GetAllUsers())
                _registeredUsers.TryAdd(user.Username, user);

            initialized = true;
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterDetails registerDetails)
    {
        if (_registeredUsers.ContainsKey(registerDetails.Username))
            return Conflict(new { Message = "Username is already taken." });

        var placeholderUser = new User
        {
            Username = registerDetails.Username,
            Password = string.Empty,
            Id = Guid.Empty,
            CardColor = "white"
        };

        _registeredUsers.TryAdd(registerDetails.Username, placeholderUser);

        var user = await _authRepo.CreateUserAsync(registerDetails);

        _registeredUsers[registerDetails.Username] = user;

        _authService.AddCookie(HttpContext, user.Id, registerDetails.RememberMe);

        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginDetails loginDetails)
    {
        try
        {
            var user = await _authRepo.GetUserByUsernameAsync(loginDetails.Username);
            if (!_authService.VerifyPassword(loginDetails.Password, user.Password))
                return Unauthorized();

            _authService.AddCookie(HttpContext, user.Id, loginDetails.RememberMe);

            return Ok();
        }
        catch (UserNotFoundException)
        {
            return Unauthorized();
        }
    }

    [HttpGet("loginResponse")]
    public IActionResult GetLoginResponse()
    {
        var userId = _authService.GetRequesterId(HttpContext);

        return Ok(userId.HasValue);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        _authService.RemoveCookie(HttpContext);
        
        return Ok();
    }

}