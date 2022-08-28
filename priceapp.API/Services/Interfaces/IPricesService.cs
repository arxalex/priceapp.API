using priceapp.API.Models;

namespace priceapp.API.Services.Interfaces;

public interface IPricesService
{
    Task<List<PriceModel>> GetPrices(int shopId, int internalFilialId, int categoryId);
    Task UpdatePricesAsync(FilialModel filial);
    Task UpdatePricesAsync(bool forceUpdate = false, bool skipSetZeroQuantity = false);
    Task SetPriceQuantitiesZeroAsync();
    Task SetPriceQuantitiesZeroAsync(int filialId);
}