using priceapp.Models;

namespace priceapp.ShopsServices.Interfaces;

public interface IAtbService
{
    Task<List<ItemShopModel>> GetItemsByCategoryAsync(int proxyCategoryId, int from, int to);
    Task<List<PriceModel>> GetPricesAsync(int categoryId, int proxyFilialId);
    Task<List<FilialModel>> GetFilialsAsync();
    Task<List<CategoryLinkModel>> GetCategoryLinksAsync();
}