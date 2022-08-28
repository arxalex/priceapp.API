using priceapp.API.Repositories.Models;

namespace priceapp.API.Repositories.Interfaces;

public interface IFilialsRepository
{
    Task<List<FilialRepositoryModel>> GetFilialsAsync(double xCord, double yCord, double radius);
    Task<List<FilialRepositoryModel>> GetFilialsAsync();
    Task<List<FilialRepositoryModel>> GetFilialsAsync(int shopId);
    Task InsertFilialsAsync(List<FilialRepositoryModel> models);
    Task<string> GetRegionAsync(string city);
}