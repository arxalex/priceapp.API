using priceapp.API.Models;

namespace priceapp.API.Services.Interfaces;

public interface IConsistsService
{
    Task<List<ConsistModel>> GetConsistsAsync();
}