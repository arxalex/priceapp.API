using priceapp.API.ShopServices.Models;

namespace priceapp.API.ShopServices.Interfaces;

public interface IForaService
{
    Task<ItemForaModel> GetItemsByCategoryAsync(int categoryId, int from, int to);
}