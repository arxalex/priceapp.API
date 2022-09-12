using priceapp.proxy.Services.Models;

namespace priceapp.proxy.Services.Interfaces;

public interface ICategoriesService
{
    Task<List<AtbCategoryModel>> GetAtbCategoriesAsync();
    Task InsertAsync(AtbCategoryModel model);
    Task<List<AtbCategoryModel>> GetAtbChildCategoriesAsync(int categoryId);
    Task InsertAsync(List<AtbCategoryModel> models);
}