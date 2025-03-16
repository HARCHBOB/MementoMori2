using MementoMori.API.Models;
using MementoMori.API.Database;
using MementoMori.API.Interfaces;
using MementoMori.API.DTOS;
using Microsoft.EntityFrameworkCore;
using MementoMori.API.Exceptions;

namespace MementoMori.API.Service;

public class AuthRepo : IAuthRepo
{
    private readonly AppDbContext _context;
    private readonly IAuthService _authService;

    public AuthRepo(AppDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    public User[] GetAllUsers() => [.. _context.Users];

    public async Task<User> CreateUserAsync(RegisterDetails registerDetails)
    {
        var existingUser = await _context.Users.AnyAsync(u => u.Username == registerDetails.Username);
        if (existingUser)
        {
            throw new Exception();
        }

        if (string.IsNullOrEmpty(registerDetails.Password))
        {
            throw new Exception();
        }

        var hashedPassword = _authService.HashPassword(registerDetails.Password);

        var user = new User
        {
            Username = registerDetails.Username,
            Password = hashedPassword,
            CardColor = "white"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            throw new UserNotFoundException();
        }
        return user;
    }

    public async Task<User> GetUserByUsernameAsync(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            throw new UserNotFoundException();
        }
        return user;
    }

    public async Task UpdateUserCardColor(Guid userId, string newColor) 
    {
        var user = await GetUserByIdAsync(userId);
        if (user == null) 
        {
            throw new UserNotFoundException();
        }
        user.CardColor = newColor;
        await _context.SaveChangesAsync();
    }
}