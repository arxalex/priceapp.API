using priceapp.Repositories.Models;

namespace priceapp.Repositories.Interfaces;

public interface IPackagesRepository
{
    Task<List<PackageRepositoryModel>> GetPackagesAsync();
    Task InsertPackageAsync(PackageRepositoryModel model);
    Task UpdatePackageAsync(PackageRepositoryModel model);
}