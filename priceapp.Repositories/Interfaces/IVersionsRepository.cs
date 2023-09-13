using priceapp.Repositories.Models;

namespace priceapp.Repositories.Interfaces;

public interface IVersionsRepository
{
    Task<VersionRepositoryModel> GetMinVersion();
}