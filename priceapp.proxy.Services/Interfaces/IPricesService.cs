using priceapp.proxy.Models;

namespace priceapp.proxy.Services.Interfaces;

public interface IPricesService
{
    Task<List<PriceModel>> GetPricesAsync(int categoryId, int shopId, int filialId);
    Task InsertAsync(List<PriceModel> models);
}