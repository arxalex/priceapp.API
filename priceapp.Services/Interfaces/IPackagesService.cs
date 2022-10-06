using priceapp.Models;

namespace priceapp.Services.Interfaces;

public interface IPackagesService
{
    Task<List<PackageModel>> GetPackagesAsync();
    Task InsertPackageAsync(PackageModel model);
    Task UpdatePackageAsync(PackageModel model);
}