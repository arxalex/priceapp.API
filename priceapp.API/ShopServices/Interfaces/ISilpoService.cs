using priceapp.API.ShopServices.Models;

namespace priceapp.API.ShopServices.Interfaces;

public interface ISilpoService
{
    Task<List<ItemSilpoModel>> GetItemsByCategoryAsync(int categoryId, int from, int to);
}