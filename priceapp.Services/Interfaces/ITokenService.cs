namespace priceapp.Services.Interfaces;

public interface ITokenService
{
    Task<bool> IsCurrentTokenActive();
    Task DeactivateTokensForUserAsync(int userId);
    Task DeactivateTokenAsync();
    Task InsertTokenAsync(int userId, string token, int expires);
}