using priceapp.API.Models;

namespace priceapp.API.Services.Interfaces;

public interface IConsistsService
{
    Task<List<ConsistModel>> GetConsistsAsync();
    Task InsertConsistAsync(ConsistModel model);
    Task UpdateConsistAsync(ConsistModel model);
}