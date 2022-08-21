using priceapp.API.Repositories.Models;

namespace priceapp.API.Repositories.Interfaces;

public interface IConsistsRepository
{
    Task<List<ConsistRepositoryModel>> GetConsistsAsync();
    Task InsertConsistAsync(ConsistRepositoryModel model);
    Task UpdateConsistAsync(ConsistRepositoryModel model);
}