using MementoMori.API.Entities;
using MementoMori.API.Data;
using MementoMori.API.Models;
using Microsoft.EntityFrameworkCore;
using MementoMori.API.Exceptions;

namespace MementoMori.API.Services;

public class AuthRepo(AppDbContext context, IAuthService authService) : IAuthRepo
{
    public User[] GetAllUsers() => [.. context.Users];

    public async Task<User> CreateUserAsync(RegisterDetails registerDetails)
    {
        var existingUser = await context.Users.AnyAsync(u => u.Username == registerDetails.Username);
        if (existingUser)
            throw new Exception();

        if (string.IsNullOrEmpty(registerDetails.Password))
            throw new Exception();

        var hashedPassword = authService.HashPassword(registerDetails.Password);

        var user = new User
        {
            Username = registerDetails.Username,
            Password = hashedPassword,
            CardColor = "white"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    public async Task<User> GetUserByIdAsync(Guid id)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Id == id) ?? throw new UserNotFoundException();
    }

    public async Task<User> GetUserByUsernameAsync(string username)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Username == username) ?? throw new UserNotFoundException();
    }

    public async Task UpdateUserCardColor(Guid userId, string newColor) 
    {
        var user = await GetUserByIdAsync(userId) ?? throw new UserNotFoundException();

        user.CardColor = newColor;
        
        await context.SaveChangesAsync();
    }
}