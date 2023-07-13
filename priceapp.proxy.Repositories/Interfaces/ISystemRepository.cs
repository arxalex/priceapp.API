namespace priceapp.proxy.Repositories.Interfaces;

public interface ISystemRepository
{
    Task<bool> IsDbConnected();
}