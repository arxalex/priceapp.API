namespace priceapp.proxy.Services.Interfaces;

public interface ISystemService
{
    Task<bool> IsDbConnected();
}