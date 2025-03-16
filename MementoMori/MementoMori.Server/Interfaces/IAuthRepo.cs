using MementoMori.Server.DTOS;
using MementoMori.Server.Models;

namespace MementoMori.Server.Interfaces;

public interface IAuthRepo
{
    Task<User> CreateUserAsync(RegisterDetails registerDetails);
    User[] GetAllUsers();
    Task<User> GetUserByIdAsync(Guid id);
    Task<User> GetUserByUsernameAsync(string username);
    Task UpdateUserCardColor(Guid userId, string newColor);
}