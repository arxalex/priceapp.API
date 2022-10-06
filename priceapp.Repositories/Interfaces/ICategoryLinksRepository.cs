using priceapp.Repositories.Models;

namespace priceapp.Repositories.Interfaces;

public interface ICategoryLinksRepository
{
    Task<List<CategoryLinkRepositoryModel>> GetCategoryLinksAsync(int shopId);
    Task<CategoryLinkRepositoryModel> GetCategoryLinkAsync(int shopId, int inShopId);
    Task InsertCategoryLinkAsync(CategoryLinkRepositoryModel model);
    Task UpdateCategoryLinkAsync(CategoryLinkRepositoryModel model);
    Task<List<CategoryLinkRepositoryModel>> GetCategoryLinksAsync(int shopId, int categoryId);
    Task InsertOrUpdateCategoryLinksAsync(List<CategoryLinkRepositoryModel> models);
}