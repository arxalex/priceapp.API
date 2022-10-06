using priceapp.Models;

namespace priceapp.Services.Interfaces;

public interface ICountriesService
{
    Task<List<CountryModel>> GetCountriesAsync();
    Task<CountryModel?> SearchCountryAsync(string search);
    Task InsertCountryAsync(CountryModel model);
    Task UpdateCountryAsync(CountryModel model);
}