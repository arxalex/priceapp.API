using priceapp.API.Models;

namespace priceapp.API.Services.Interfaces;

public interface ICategoriesService
{
    Task<List<CategoryModel>> GetChildCategoriesAsync(int baseCategoryId);
    Task<List<CategoryLinkModel>> GetCategoryLinksByShopAsync(int shopId);
    Task<List<CategoryModel>> GetCategoriesAsync();
    Task<CategoryModel> GetCategoryByShopAndInShopIdAsync(int shopId, int inShopId);
}