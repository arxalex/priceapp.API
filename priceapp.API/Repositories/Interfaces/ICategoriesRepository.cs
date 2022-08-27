using priceapp.API.Repositories.Models;

namespace priceapp.API.Repositories.Interfaces;

public interface ICategoriesRepository
{
    Task<List<CategoryRepositoryModel>> GetChildCategoriesAsync(int categoryId);
    Task<List<CategoryLinkRepositoryModel>> GetCategoryLinksAsync(int shopId);
    Task<List<CategoryRepositoryModel>> GetCategoriesAsync();
    Task<CategoryRepositoryModel> GetCategoryAsync(int shopId, int inShopId);
    Task<CategoryLinkRepositoryModel> GetCategoryLinkAsync(int shopId, int inShopId);
    Task InsertCategoryAsync(CategoryRepositoryModel model);
    Task InsertCategoryLinkAsync(CategoryLinkRepositoryModel model);
    Task UpdateCategoryAsync(CategoryRepositoryModel model);
    Task UpdateCategoryLinkAsync(CategoryLinkRepositoryModel model);
    Task<List<CategoryLinkRepositoryModel>> GetCategoryLinksAsync(int shopId, int categoryId);
    Task<List<CategoryRepositoryModel>> GetBaseCategoriesAsync();
}