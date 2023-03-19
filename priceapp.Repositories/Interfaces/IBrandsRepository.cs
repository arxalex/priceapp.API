using priceapp.Repositories.Models;

namespace priceapp.Repositories.Interfaces;

public interface IBrandsRepository
{
    Task<List<BrandRepositoryModel>> GetBrandsAsync();
    Task<List<BrandRepositoryModel>> GetBrandsByKeywordsAsync(List<string> keywords);
    Task InsertBrandAsync(BrandRepositoryModel model);
    Task UpdateBrandAsync(BrandRepositoryModel model);
    Task<List<BrandAlertRepositoryModel>> GetBrandAlertsAsync(int brandId);
}