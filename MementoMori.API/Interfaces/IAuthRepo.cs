using MementoMori.API.DTOS;
using MementoMori.API.Models;

namespace MementoMori.API.Interfaces;

public interface IAuthRepo
{
    Task<User> CreateUserAsync(RegisterDetails registerDetails);
    User[] GetAllUsers();
    Task<User> GetUserByIdAsync(Guid id);
    Task<User> GetUserByUsernameAsync(string username);
    Task UpdateUserCardColor(Guid userId, string newColor);
}