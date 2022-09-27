using priceapp.API.Models;

namespace priceapp.API.Services.Interfaces;

public interface IPricesService
{
    Task<List<PriceModel>> GetPricesAsync(int shopId, int internalFilialId, int categoryId);
    Task UpdatePricesAsync(FilialModel filial);
    Task StartUpdatePricesTasksAsync(bool forceUpdate = false, bool skipSetZeroQuantity = false);
    Task SetPriceQuantitiesZeroAsync();
    Task SetPriceQuantitiesZeroAsync(int filialId);
    Task RefactorPricesAsync();
    Task<List<PriceModel>> GetPricesAsync();
    Task UpdatePricesAsync(bool forceUpdate = false, bool skipSetZeroQuantity = false);
}