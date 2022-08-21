using priceapp.API.Models;

namespace priceapp.API.Services.Interfaces;

public interface ICategoriesService
{
    Task<List<CategoryModel>> GetChildCategoriesAsync(int baseCategoryId);
    Task<List<CategoryLinkModel>> GetCategoryLinksAsync(int shopId);
    Task<List<CategoryModel>> GetCategoriesAsync();
    Task<CategoryModel> GetCategoryAsync(int shopId, int inShopId);
    Task<CategoryLinkModel> GetCategoryLinkAsync(int shopId, int inShopId);
    Task InsertCategoryAsync(CategoryModel model);
    Task InsertCategoryLinkAsync(CategoryLinkModel model);
    Task UpdateCategoryAsync(CategoryModel model);
    Task UpdateCategoryLinkAsync(CategoryLinkModel model);
}