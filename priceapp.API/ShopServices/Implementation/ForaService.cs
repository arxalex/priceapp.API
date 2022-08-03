using priceapp.API.ShopServices.Interfaces;
using priceapp.API.ShopServices.Models;

namespace priceapp.API.ShopServices.Implementation;

public class ForaService : IForaService
{
    public Task<ItemForaModel> GetItemsByCategoryAsync(int categoryId, int from, int to)
    {
        throw new NotImplementedException();
    }
}