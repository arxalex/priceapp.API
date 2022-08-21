using priceapp.API.Repositories.Models;

namespace priceapp.API.Repositories.Interfaces;

public interface IPackagesRepository
{
    Task<List<PackageRepositoryModel>> GetPackagesAsync();
    Task InsertPackageAsync(PackageRepositoryModel model);
    Task UpdatePackageAsync(PackageRepositoryModel model);
}