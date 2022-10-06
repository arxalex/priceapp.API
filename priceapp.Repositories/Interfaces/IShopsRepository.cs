using priceapp.Repositories.Models;

namespace priceapp.Repositories.Interfaces;

public interface IShopsRepository
{
    Task<List<ShopRepositoryModel>> GetShopsAsync();
}