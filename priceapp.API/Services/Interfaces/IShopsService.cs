using priceapp.API.Models;

namespace priceapp.API.Services.Interfaces;

public interface IShopsService
{
    Task<List<ShopModel>> GetShopsAsync();
}