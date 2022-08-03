using priceapp.API.ShopServices.Models;

namespace priceapp.API.ShopServices.Interfaces;

public interface IAtbService
{
    Task<ItemAtbModel> GetItemsByCategoryAsync(int categoryId, int from, int to);
}