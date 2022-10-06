using priceapp.Repositories.Models;

namespace priceapp.Repositories.Interfaces;

public interface IUsersRepository
{
    Task<UserRepositoryModel> GetUserByUsernameAsync(string username);
    Task<UserRepositoryModel> GetUserByEmailAsync(string email);
    Task RegisterUserAsync(string username, string email, string password, int role);
    Task<bool> IsUserExistsAsync(string username, string email);
    Task<bool> IsUserExistsAsync(int id);
    Task<UserRepositoryModel> GetUserByIdAsync(int id);
    Task UpdateUserRole(int id, int role);
    Task ChangePasswordAsync(int id, string password);
}