using priceapp.API.ShopServices.Interfaces;
using priceapp.API.ShopServices.Models;

namespace priceapp.API.ShopServices.Implementation;

public class SilpoService : ISilpoService
{
    public Task<List<ItemSilpoModel>> GetItemsByCategoryAsync(int categoryId, int from, int to)
    {
        throw new NotImplementedException();
    }
}