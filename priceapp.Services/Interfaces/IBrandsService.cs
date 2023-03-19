using priceapp.Models;

namespace priceapp.Services.Interfaces;

public interface IBrandsService
{
    Task<List<BrandModel>> GetBrandsAsync();
    Task<BrandModel?> SearchBrandAsync(string search);
    Task InsertBrandAsync(BrandModel model);
    Task UpdateBrandAsync(BrandModel model);
    Task<List<BrandAlertModel>> GetBrandAlertsAsync(int brandId);
}