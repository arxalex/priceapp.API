using priceapp.API.Repositories.Models;

namespace priceapp.API.Repositories.Interfaces;

public interface ICountriesRepository
{
    Task<List<CountryRepositoryModel>> GetCountriesAsync();
    Task<List<CountryRepositoryModel>> GetCountriesByKeywordsAsync(List<string> keywords);
}