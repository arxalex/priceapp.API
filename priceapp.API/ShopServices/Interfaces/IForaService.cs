using priceapp.API.Models;

namespace priceapp.API.ShopServices.Interfaces;

public interface IForaService
{
    Task<List<ItemShopModel>> GetItemsByCategoryAsync(int categoryId, int from, int to, int filialId = 310);
}