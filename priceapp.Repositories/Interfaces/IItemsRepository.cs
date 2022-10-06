using priceapp.Repositories.Models;

namespace priceapp.Repositories.Interfaces;

public interface IItemsRepository
{
    public Task<List<ItemRepositoryModel>> GetItemsAsync();
    public Task<ItemRepositoryModel> GetItemAsync(int id);

    public Task<List<ItemRepositoryModel>> GetItemsAsync(IEnumerable<string> keywords,
        IEnumerable<int> categoryIds);

    Task<List<ItemRepositoryModel>> GetItemsAsync(List<string> keywords);

    Task<List<ItemExtendedRepositoryModel>> GetItemExtendedAsync(IEnumerable<int> categoryIds, int from,
        int to);

    Task<List<ItemExtendedRepositoryModel>> GetItemsExtendedAsync(IEnumerable<int> categoryIds,
        IEnumerable<int> filialIds,
        int from, int to);

    Task<List<ItemExtendedRepositoryModel>> SearchItemsExtendedAsync(List<string> search);

    Task<List<ItemExtendedRepositoryModel>> SearchItemsExtendedAsync(List<string> search,
        IEnumerable<int> filialIds);

    Task<ItemExtendedRepositoryModel> GetItemExtendedAsync(int id);
    Task<ItemExtendedRepositoryModel> GetItemExtendedAsync(int id, IEnumerable<int> filialIds);
    Task<List<ItemLinkRepositoryModel>> GetItemLinksAsync(int shopId);
    Task InsertItemAsync(ItemRepositoryModel model);
    Task UpdateItemAsync(ItemRepositoryModel model);
    Task InsertItemLinkAsync(ItemLinkRepositoryModel model);
    Task<ItemRepositoryModel> GetLastInsertedItemAsync();
    Task<List<ItemLinkRepositoryModel>> GetItemLinksAsync(int shopId, IEnumerable<int> categoryIds);
}