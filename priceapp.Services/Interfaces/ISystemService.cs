namespace priceapp.Services.Interfaces;

public interface ISystemService
{
    Task<bool> IsDbConnected();
    Task<bool> IsNeedUpdate(string version);
}