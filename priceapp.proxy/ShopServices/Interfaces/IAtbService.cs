using priceapp.proxy.Services.Models;

namespace priceapp.proxy.ShopServices.Interfaces;

public interface IAtbService
{
    Task<List<PriceModel>> GetPricesAsync(int categoryId, int filialId);
    Task<List<AtbFilialModel>> GetFilialsAsync();
    Task<List<AtbCategoryModel>> GetCategoriesAsync(bool insertParent = true);
    Task<List<AtbItemModel>> GetItemsAsync(int internalCategoryId, int internalFilialId = 1154);
}