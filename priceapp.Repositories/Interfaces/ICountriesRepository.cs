using priceapp.Repositories.Models;

namespace priceapp.Repositories.Interfaces;

public interface ICountriesRepository
{
    Task<List<CountryRepositoryModel>> GetCountriesAsync();
    Task<List<CountryRepositoryModel>> GetCountriesByKeywordsAsync(List<string> keywords);
    Task InsertCountryAsync(CountryRepositoryModel model);
    Task UpdateCountryAsync(CountryRepositoryModel model);
}