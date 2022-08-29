using priceapp.API.Models;

namespace priceapp.API.ShopServices.Interfaces;

public interface IAtbService
{
    Task<List<ItemShopModel>> GetItemsByCategoryAsync(int proxyCategoryId, int from, int to);
    Task<List<PriceModel>> GetPricesAsync(int categoryId, int proxyFilialId, int from = 0, int to = 10000);
    Task<List<FilialModel>> GetFilialsAsync();
    Task<List<CategoryLinkModel>> GetCategoryLinksAsync();
}