using priceapp.API.Models;

namespace priceapp.API.ShopServices.Interfaces;

public interface ISilpoService
{
    Task<List<ItemShopModel>> GetItemsByCategoryAsync(int categoryId, int from, int to, int filialId = 2043);
}