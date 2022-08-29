using priceapp.API.Models;

namespace priceapp.API.Services.Interfaces;

public interface ICategoryLinksService
{
    Task<List<CategoryLinkModel>> GetCategoryLinksFromShopsAsync(int shopId);
    Task<List<CategoryLinkModel>> GetCategoryLinksAsync(int shopId, int categoryId);
    Task UpdateCategoryLinkAsync(CategoryLinkModel model);
    Task InsertCategoryLinkAsync(CategoryLinkModel model);
    Task<CategoryLinkModel> GetCategoryLinkAsync(int shopId, int inShopId);
    Task<List<CategoryLinkModel>> GetCategoryLinksAsync(int shopId);
}