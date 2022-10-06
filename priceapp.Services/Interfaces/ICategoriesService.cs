using priceapp.Models;

namespace priceapp.Services.Interfaces;

public interface ICategoriesService
{
    Task<List<CategoryModel>> GetChildCategoriesAsync(int baseCategoryId);
    Task<List<CategoryModel>> GetCategoriesAsync();
    Task<CategoryModel> GetCategoryAsync(int shopId, int inShopId);
    Task InsertCategoryAsync(CategoryModel model);
    Task UpdateCategoryAsync(CategoryModel model);
    Task<List<CategoryModel>> GetBaseCategoriesAsync();
}