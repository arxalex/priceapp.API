using priceapp.proxy.Repositories.Models;

namespace priceapp.proxy.Repositories.Interfaces;

public interface IPricesRepository
{
    Task<List<PriceRepositoryModel>> GetPrices(IEnumerable<int> categoryIds, int shopId, int filialId);
    Task InsertOrUpdateAsync(List<PriceRepositoryModel> models);
}