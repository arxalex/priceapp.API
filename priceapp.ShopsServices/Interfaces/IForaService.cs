using priceapp.Models;

namespace priceapp.ShopsServices.Interfaces;

public interface IForaService
{
    Task<List<ItemShopModel>> GetItemsByCategoryAsync(int internalCategoryId, int from, int to, int internalFilialId = 310);
    Task<List<PriceModel>> GetPricesAsync(int categoryId, int internalFilialId, int filialId, int from = 0,
        int to = 10000);
    Task<List<FilialModel>> GetFilialsAsync();
    Task<List<CategoryLinkModel>> GetCategoryLinksAsync(int internalFilialId = 310);
}