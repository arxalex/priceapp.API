using priceapp.Models;

namespace priceapp.Services.Interfaces;

public interface IFilialsService
{
    Task<List<FilialModel>> GetFilialsAsync(double xCord, double yCord, double radius);
    Task<List<FilialModel>> GetFilialsAsync();
    Task<string> GetRegionAsync(string city);
    Task InsertFilialsAsync(List<FilialModel> filialsToInsert);
}