namespace priceapp.Repositories.Interfaces;

public interface ISystemRepository
{
    Task<bool> IsDbConnected();
}