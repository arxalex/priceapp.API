using priceapp.API.Repositories.Models;

namespace priceapp.API.Repositories.Interfaces;

public interface IFilialsRepository
{
    Task<List<FilialRepositoryModel>> GetFilialsByLocationAsync(double xCord, double yCord, double radius);
}