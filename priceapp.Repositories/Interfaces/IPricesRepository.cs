using priceapp.Repositories.Models;

namespace priceapp.Repositories.Interfaces;

public interface IPricesRepository
{
    Task InsertOrUpdatePricesAsync(List<PriceRepositoryModel> models);
    Task SetPriceQuantitiesZeroAsync();
    Task SetPriceQuantitiesZeroAsync(int filialId);
    Task InsertOrUpdatePricesHistoryAsync(List<PriceHistoryRepositoryModel> models);
    Task<int?> GetMaxFilialIdToday();
    Task<List<PriceRepositoryModel>> GetPricesAsync();
}