using priceapp.API.ShopServices.Interfaces;
using priceapp.API.ShopServices.Models;

namespace priceapp.API.ShopServices.Implementation;

public class AtbService : IAtbService
{
    public Task<ItemAtbModel> GetItemsByCategoryAsync(int categoryId, int from, int to)
    {
        throw new NotImplementedException();
    }
}