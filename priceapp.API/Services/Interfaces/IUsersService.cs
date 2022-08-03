using priceapp.API.Models;

namespace priceapp.API.Services.Interfaces;

public interface IUsersService
{
    Task<(UserModel, string token, int expires)> GetUserAndTokenByEmailAsync(string email, string password);
    Task<(UserModel, string token, int expires)> GetUserAndTokenByUsernameAsync(string username, string password);
    Task<(UserModel, string token, int expires)> GetUserAndTokenAsync(int userId, string password);
    Task RegisterUserAsync(string username, string email, string password);
    Task<UserModel> GetUserByIdAsync(int userId);
    Task VerifyUserEmailAsync(int userId, string token);
    Task ChangePasswordAsync(int userId, string password, string passwordOld);
}