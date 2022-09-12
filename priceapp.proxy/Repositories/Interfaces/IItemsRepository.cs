using priceapp.proxy.Repositories.Models;

namespace priceapp.proxy.Repositories.Interfaces;

public interface IItemsRepository
{
    Task<List<AtbItemRepositoryModel>> GetAtbItemsAsync(IEnumerable<int> categoryIds, int from, int to);
    Task<List<AtbItemRepositoryModel>> GetAtbItemsAsync();
    Task InsertAsync(AtbItemRepositoryModel model);
    Task InsertOrUpdateAsync(List<AtbItemRepositoryModel> models);
}