using priceapp.API.Models;

namespace priceapp.API.Services.Interfaces;

public interface IFilialsService
{
    Task<List<FilialModel>> GetFilialsAsync(double xCord, double yCord, double radius);
    Task<List<FilialModel>> GetFilialsAsync();
    Task<List<FilialModel>> GetFilialsAsync(int shopId);
    Task ActualizeFilialsAsync();
    Task<string> GetRegionAsync(string city);
}