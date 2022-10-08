using priceapp.proxy.Repositories.Models;

namespace priceapp.proxy.Repositories.Interfaces;

public interface ICategoriesRepository
{
    Task<List<AtbCategoryRepositoryModel>> GetAtbChildCategoriesAsync(int categoryId);
    Task<List<AtbCategoryRepositoryModel>> GetAtbCategoriesAsync();
    Task InsertAsync(AtbCategoryRepositoryModel model);
    Task InsertOrUpdateAsync(List<AtbCategoryRepositoryModel> models);
    Task<List<AtbCategoryRepositoryModel>> GetAtbBaseCategoriesAsync();
}