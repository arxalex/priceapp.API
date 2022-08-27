using priceapp.API.Models;

namespace priceapp.API.Services.Interfaces;

public interface IPricesService
{
    Task<List<PriceModel>> GetPrices(int shopId, int internalFilialId, int categoryId);
}