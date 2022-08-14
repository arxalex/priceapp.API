using priceapp.API.Repositories.Models;

namespace priceapp.API.Repositories.Interfaces;

public interface IItemsRepository
{
    public Task<List<ItemRepositoryModel>> GetItemsAsync();
    public Task<ItemRepositoryModel> GetItemByIdAsync(int id);

    public Task<List<ItemRepositoryModel>> GetItemsByKeywordsAndCategoryAsync(IEnumerable<string> keywords,
        IEnumerable<int> categoryIds);

    Task<List<ItemRepositoryModel>> GetItemsByKeywordsAsync(List<string> keywords);

    Task<List<ItemExtendedRepositoryModel>> GetItemExtendedByCategoriesAsync(IEnumerable<int> categoryIds, int from,
        int to);

    Task<List<ItemExtendedRepositoryModel>> GetItemsExtendedByCategoriesAndFilialsAsync(IEnumerable<int> categoryIds,
        IEnumerable<int> filialIds,
        int from, int to);

    Task<List<ItemExtendedRepositoryModel>> SearchItemsExtendedAsync(List<string> search);

    Task<List<ItemExtendedRepositoryModel>> SearchItemsExtendedByLocationAsync(List<string> search,
        IEnumerable<int> filialIds);

    Task<ItemExtendedRepositoryModel> GetItemExtendedAsync(int id);
    Task<ItemExtendedRepositoryModel> GetItemExtendedByLocationAsync(int id, IEnumerable<int> filialIds);
    Task<List<ItemLinkRepositoryModel>> GetItemLinksByShopAsync(int shopId);
}