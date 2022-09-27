using priceapp.API.Repositories.Models;

namespace priceapp.API.Repositories.Interfaces;

public interface IPricesRepository
{
    Task InsertOrUpdatePricesAsync(List<PriceRepositoryModel> models);
    Task SetPriceQuantitiesZeroAsync();
    Task SetPriceQuantitiesZeroAsync(int filialId);
    Task InsertOrUpdatePricesHistoryAsync(List<PriceHistoryRepositoryModel> models);
    Task<int?> GetMaxFilialIdToday();
    Task<List<PriceRepositoryModel>> GetPricesAsync();
}