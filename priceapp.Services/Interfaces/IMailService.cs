namespace priceapp.Services.Interfaces;

public interface IMailService
{
    Task SendRegistrationConfirmEmailAsync(int userId, string email, string token);
}