using priceapp.Models;

namespace priceapp.Services.Interfaces;

public interface IShopsService
{
    Task<List<ShopModel>> GetShopsAsync();
}