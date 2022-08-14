using priceapp.API.Repositories.Models;

namespace priceapp.API.Repositories.Interfaces;

public interface IShopsRepository
{
    Task<List<ShopRepositoryModel>> GetShopsAsync();
}