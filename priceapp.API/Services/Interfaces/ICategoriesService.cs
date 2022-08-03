using priceapp.API.Models;

namespace priceapp.API.Services.Interfaces;

public interface ICategoriesService
{
    Task<List<CategoryModel>> GetChildCategoriesAsync(int baseCategoryId);
}