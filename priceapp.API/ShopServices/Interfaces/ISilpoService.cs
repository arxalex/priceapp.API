using priceapp.API.Models;

namespace priceapp.API.ShopServices.Interfaces;

public interface ISilpoService
{
    Task<List<ItemShopModel>> GetItemsByCategoryAsync(int internalCategoryId, int from, int to, int internalFilialId = 2043);
    Task<List<PriceModel>> GetPrices(int categoryId, int internalFilialId, int from = 0, int to = 10000);
}