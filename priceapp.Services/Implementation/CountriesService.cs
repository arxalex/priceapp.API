using AutoMapper;
using priceapp.Models;
using priceapp.Repositories.Interfaces;
using priceapp.Repositories.Models;
using priceapp.Services.Interfaces;
using priceapp.Utils;

namespace priceapp.Services.Implementation;

public class CountriesService : ICountriesService
{
    private readonly ICountriesRepository _countriesRepository;
    private readonly IMapper _mapper;

    public CountriesService(IMapper mapper, ICountriesRepository countriesRepository)
    {
        _mapper = mapper;
        _countriesRepository = countriesRepository;
    }

    public async Task<List<CountryModel>> GetCountriesAsync()
    {
        return _mapper.Map<List<CountryModel>>(await _countriesRepository.GetCountriesAsync());
    }

    public async Task<CountryModel?> SearchCountryAsync(string search)
    {
        var keywords = StringUtil.NameToKeywords(search);
        var items = _mapper.Map<List<CountryModel>>(await _countriesRepository.GetCountriesByKeywordsAsync(keywords));

        var rates = StringUtil.RateItemsByKeywords(search, items.Select(x => (x.Id, x.Label)).ToList());

        return items.OrderByDescending(x => rates[x.Id]).ToList().FirstOrDefault();
    }

    public async Task InsertCountryAsync(CountryModel model)
    {
        await _countriesRepository.InsertCountryAsync(_mapper.Map<CountryRepositoryModel>(model));
    }

    public async Task UpdateCountryAsync(CountryModel model)
    {
        await _countriesRepository.UpdateCountryAsync(_mapper.Map<CountryRepositoryModel>(model));
    }
}