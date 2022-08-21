using priceapp.API.Repositories.Models;

namespace priceapp.API.Repositories.Interfaces;

public interface IBrandsRepository
{
    Task<List<BrandRepositoryModel>> GetBrandsAsync();
    Task<List<BrandRepositoryModel>> GetBrandsByKeywordsAsync(List<string> keywords);
    Task InsertBrandAsync(BrandRepositoryModel model);
    Task UpdateBrandAsync(BrandRepositoryModel model);
}