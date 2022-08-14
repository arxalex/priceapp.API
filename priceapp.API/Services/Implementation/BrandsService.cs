using AutoMapper;
using priceapp.API.Models;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Services.Interfaces;
using priceapp.API.Utils;

namespace priceapp.API.Services.Implementation;

public class BrandsService : IBrandsService
{
    private readonly IBrandsRepository _brandsRepository;
    private readonly IMapper _mapper;

    public BrandsService(IMapper mapper, IBrandsRepository brandsRepository)
    {
        _mapper = mapper;
        _brandsRepository = brandsRepository;
    }

    public async Task<List<BrandModel>> GetBrandsAsync()
    {
        return _mapper.Map<List<BrandModel>>(await _brandsRepository.GetBrandsAsync());
    }

    public async Task<BrandModel?> SearchBrandAsync(string search)
    {
        var keywords = StringUtil.NameToKeywords(search);
        var items = _mapper.Map<List<BrandModel>>(await _brandsRepository.GetBrandsByKeywordsAsync(keywords));

        var rates = StringUtil.RateItemsByKeywords(search, items.Select(x => (x.Id, x.Label)).ToList());

        return items.OrderByDescending(x => rates[x.Id]).ToList().FirstOrDefault();
    }
}