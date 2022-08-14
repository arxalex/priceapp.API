using AutoMapper;
using priceapp.API.Models;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Services.Interfaces;
using priceapp.API.Utils;

namespace priceapp.API.Services.Implementation;

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

        var rates = StringUtil.RateItemsByKeywords(search, items.Select(x => x.Label).ToList());

        return items.OrderBy(x => rates[x.Id]).ToList().FirstOrDefault();
    }
}