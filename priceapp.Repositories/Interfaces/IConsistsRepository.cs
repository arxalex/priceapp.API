using priceapp.Repositories.Models;

namespace priceapp.Repositories.Interfaces;

public interface IConsistsRepository
{
    Task<List<ConsistRepositoryModel>> GetConsistsAsync();
    Task InsertConsistAsync(ConsistRepositoryModel model);
    Task UpdateConsistAsync(ConsistRepositoryModel model);
}