using priceapp.API.Models;

namespace priceapp.API.Services.Interfaces;

public interface IBrandsService
{
    Task<List<BrandModel>> GetBrandsAsync();
    Task<BrandModel?> SearchBrandAsync(string search);
}