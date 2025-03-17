using MementoMori.API.Models;
using MementoMori.API.Entities;

namespace MementoMori.API.Services;

public interface IAuthRepo
{
    Task<User> CreateUserAsync(RegisterDetails registerDetails);
    User[] GetAllUsers();
    Task<User> GetUserByIdAsync(Guid id);
    Task<User> GetUserByUsernameAsync(string username);
    Task UpdateUserCardColor(Guid userId, string newColor);
}