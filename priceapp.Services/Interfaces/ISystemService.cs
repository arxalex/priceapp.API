namespace priceapp.Services.Interfaces;

public interface ISystemService
{
    Task<bool> IsDbConnected();
}