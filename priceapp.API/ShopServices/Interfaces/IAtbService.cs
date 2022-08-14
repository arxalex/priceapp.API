using priceapp.API.Models;

namespace priceapp.API.ShopServices.Interfaces;

public interface IAtbService
{
    Task<List<ItemShopModel>> GetItemsByCategoryAsync(int categoryId, int from, int to);
}