namespace priceapp.Repositories.Interfaces;

public interface ITokensRepository
{
    Task InsertEmailConfirmationTokenAsync(int userId, string token);
    Task<bool> IsConfirmEmailTokenExistsAsync(int userId);
    Task CloseConfirmEmailTokenAsync(int userId, string token);
    Task<bool> IsJWTTokenExistsAsync(string token);
    Task DeleteTokensForUserAsync(int userId);
    Task DeleteTokenAsync(string token);
    Task InsertTokenAsync(int userId, string token, int expires);
}