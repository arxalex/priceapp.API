using priceapp.Models;

namespace priceapp.Services.Interfaces;

public interface IPricesService
{
    Task SetPriceQuantitiesZeroAsync();
    Task SetPriceQuantitiesZeroAsync(int filialId);
    Task RefactorPricesAsync();
    Task<List<PriceModel>> GetPricesAsync();
    Task<int?> GetMaxFilialIdToday();
    Task InsertOrUpdatePricesAsync(List<PriceModel> prices);
    Task InsertOrUpdatePricesHistoryAsync(List<PriceHistoryModel> pricesHistory);
    Task<List<PriceModel>> GetPricesAsync(int itemId, double xCord, double yCord, double radius);
    Task<List<PriceModel>> GetPricesAsync(IEnumerable<int> itemIds, double xCord, double yCord, double radius);
}