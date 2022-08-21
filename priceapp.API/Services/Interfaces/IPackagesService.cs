using priceapp.API.Models;

namespace priceapp.API.Services.Interfaces;

public interface IPackagesService
{
    Task<List<PackageModel>> GetPackagesAsync();
    Task InsertPackageAsync(PackageModel model);
    Task UpdatePackageAsync(PackageModel model);
}