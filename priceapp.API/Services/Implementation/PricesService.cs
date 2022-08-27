using AutoMapper;
using priceapp.API.Models;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Repositories.Models;
using priceapp.API.Services.Interfaces;
using priceapp.API.ShopServices.Interfaces;

namespace priceapp.API.Services.Implementation;

public class PricesService : IPricesService
{
    private readonly ISilpoService _silpoService;
    private readonly IForaService _foraService;
    private readonly IAtbService _atbService;
    private readonly IPricesRepository _pricesRepository;
    private readonly IMapper _mapper;
    private readonly IFilialsService _filialsService;
    private readonly ICategoriesService _categoriesService;

    public PricesService(ISilpoService silpoService, IForaService foraService, IAtbService atbService, IPricesRepository pricesRepository, IMapper mapper, IFilialsService filialsService, ICategoriesService categoriesService)
    {
        _silpoService = silpoService;
        _foraService = foraService;
        _atbService = atbService;
        _pricesRepository = pricesRepository;
        _mapper = mapper;
        _filialsService = filialsService;
        _categoriesService = categoriesService;
    }

    public async Task<List<PriceModel>> GetPrices(int shopId, int internalFilialId, int categoryId)
    {
        var prices = shopId switch
        {
            1 => await _silpoService.GetPrices(categoryId, internalFilialId),
            2 => await _foraService.GetPrices(categoryId, internalFilialId),
            3 => await _atbService.GetPrices(categoryId, internalFilialId),
            _ => new List<PriceModel>()
        };

        return prices;
    }

    public async Task UpdatePricesAsync(FilialModel filial)
    {
        var categories = await _categoriesService.GetBaseCategoriesAsync();
        var prices = new List<PriceModel>();
        foreach (var category in categories)
        {
            prices.AddRange(await GetPrices(filial.ShopId, filial.InShopId, category.Id));
        }

        var pricesHistory = prices.Select(x => new PriceHistoryModel()
        {
            Id = -1,
            Date = DateTime.Now,
            FilialId = x.FilialId,
            ItemId = x.ItemId,
            Price = x.Price,
            ShopId = x.ShopId
        });

        await _pricesRepository.SetPriceQuantitiesZeroAsync();
        await _pricesRepository.InsertOrUpdatePricesAsync(_mapper.Map<List<PriceRepositoryModel>>(prices));
        await _pricesRepository.InsertOrUpdatePricesHistoryAsync(
            _mapper.Map<List<PriceHistoryRepositoryModel>>(pricesHistory));
    }
}