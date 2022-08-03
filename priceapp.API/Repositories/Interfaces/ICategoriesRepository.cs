using priceapp.API.Repositories.Models;

namespace priceapp.API.Repositories.Interfaces;

public interface ICategoriesRepository
{
    Task<List<CategoryRepositoryModel>> GetChildCategoriesAsync(int categoryId);
}