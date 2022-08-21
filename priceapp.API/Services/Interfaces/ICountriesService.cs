using priceapp.API.Models;

namespace priceapp.API.Services.Interfaces;

public interface ICountriesService
{
    Task<List<CountryModel>> GetCountriesAsync();
    Task<CountryModel?> SearchCountryAsync(string search);
    Task InsertCountryAsync(CountryModel model);
    Task UpdateCountryAsync(CountryModel model);
}