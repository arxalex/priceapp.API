using priceapp.API.Models;

namespace priceapp.API.ShopServices.Interfaces;

public interface IAtbService
{
    Task<List<ItemShopModel>> GetItemsByCategoryAsync(int proxyCategoryId, int from, int to);
    Task<List<PriceModel>> GetPrices(int categoryId, int filialId, int from = 0, int to = 10000);
}