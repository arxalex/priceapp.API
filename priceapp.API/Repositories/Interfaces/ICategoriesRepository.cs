using priceapp.API.Repositories.Models;

namespace priceapp.API.Repositories.Interfaces;

public interface ICategoriesRepository
{
    Task<List<CategoryRepositoryModel>> GetChildCategoriesAsync(int categoryId);
    Task<List<CategoryLinkRepositoryModel>> GetCategoryLinksByShopAsync(int shopId);
    Task<List<CategoryRepositoryModel>> GetCategoriesAsync();
    Task<CategoryRepositoryModel> GetCategoryByShopAndInShopIdAsync(int shopId, int inShopId);
}