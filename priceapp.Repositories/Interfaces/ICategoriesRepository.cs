using priceapp.Repositories.Models;

namespace priceapp.Repositories.Interfaces;

public interface ICategoriesRepository
{
    Task<List<CategoryRepositoryModel>> GetChildCategoriesAsync(int categoryId);
    Task<List<CategoryRepositoryModel>> GetCategoriesAsync();
    Task<CategoryRepositoryModel> GetCategoryAsync(int shopId, int inShopId);
    Task InsertCategoryAsync(CategoryRepositoryModel model);
    Task UpdateCategoryAsync(CategoryRepositoryModel model);
    Task<List<CategoryRepositoryModel>> GetBaseCategoriesAsync();
}