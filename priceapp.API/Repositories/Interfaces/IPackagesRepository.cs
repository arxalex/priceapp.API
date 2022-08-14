using priceapp.API.Repositories.Models;

namespace priceapp.API.Repositories.Interfaces;

public interface IPackagesRepository
{
    Task<List<PackageRepositoryModel>> GetPackagesAsync();
}