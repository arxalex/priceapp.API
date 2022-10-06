using priceapp.Models;

namespace priceapp.Services.Interfaces;

public interface IConsistsService
{
    Task<List<ConsistModel>> GetConsistsAsync();
    Task InsertConsistAsync(ConsistModel model);
    Task UpdateConsistAsync(ConsistModel model);
}