using AutoMapper;
using priceapp.Models;
using priceapp.Repositories.Interfaces;
using priceapp.Repositories.Models;
using priceapp.Services.Interfaces;

namespace priceapp.Services.Implementation;

public class PricesService : IPricesService
{
    private readonly IPricesRepository _pricesRepository;
    private readonly IFilialsService _filialsService;
    private readonly IMapper _mapper;

    public PricesService(IPricesRepository pricesRepository, IMapper mapper, IFilialsService filialsService)
    {
        _pricesRepository = pricesRepository;
        _mapper = mapper;
        _filialsService = filialsService;
    }

    public async Task SetPriceQuantitiesZeroAsync()
    {
        await _pricesRepository.SetPriceQuantitiesZeroAsync();
    }

    public async Task SetPriceQuantitiesZeroAsync(int filialId)
    {
        await _pricesRepository.SetPriceQuantitiesZeroAsync(filialId);
    }

    public async Task RefactorPricesAsync()
    {
        var prices = (await GetPricesAsync()).GroupBy(x => x.ItemId);
        var pricesToUpdate = new List<PriceModel>();
        foreach (var priceGroup in prices)
        {
            var priceSum = 0.0;
            var i = 0;

            foreach (var price in priceGroup)
            {
                if (price.PriceFactor != null)
                {
                    continue;
                }

                priceSum += price.Price;
                i++;
            }

            var priceAvg = priceSum / i;

            foreach (var price in priceGroup)
            {
                if (price.PriceFactor != null)
                {
                    continue;
                }

                var factor = priceAvg / price.Price;
                switch (factor)
                {
                    case > 6:
                        price.PriceFactor = 10;
                        pricesToUpdate.Add(price);
                        break;
                    case < 0.4:
                        price.PriceFactor = 0.1;
                        pricesToUpdate.Add(price);
                        break;
                }
            }
        }

        await _pricesRepository.InsertOrUpdatePricesAsync(_mapper.Map<List<PriceRepositoryModel>>(pricesToUpdate));
    }

    public async Task<List<PriceModel>> GetPricesAsync()
    {
        return _mapper.Map<List<PriceModel>>(await _pricesRepository.GetPricesAsync());
    }
    
    public async Task<List<PriceModel>> GetPricesAsync(int itemId, double xCord, double yCord, double radius)
    {
        var filials = await _filialsService.GetFilialsAsync(xCord, yCord, radius);
        return _mapper.Map<List<PriceModel>>(await _pricesRepository.GetPricesAsync(itemId, filials.Select(x => x.Id)));
    }

    public async Task<List<PriceModel>> GetPricesAsync(IEnumerable<int> itemIds, double xCord, double yCord, double radius)
    {
        var filials = await _filialsService.GetFilialsAsync(xCord, yCord, radius);
        return _mapper.Map<List<PriceModel>>(await _pricesRepository.GetPricesAsync(itemIds, filials.Select(x => x.Id)));
    }
    
    public async Task<int?> GetMaxFilialIdToday()
    {
        return await _pricesRepository.GetMaxFilialIdToday();
    }

    public async Task InsertOrUpdatePricesAsync(List<PriceModel> prices)
    {
        await _pricesRepository.InsertOrUpdatePricesAsync(_mapper.Map<List<PriceRepositoryModel>>(prices));
    }

    public async Task InsertOrUpdatePricesHistoryAsync(List<PriceHistoryModel> pricesHistory)
    {
        await _pricesRepository.InsertOrUpdatePricesHistoryAsync(
            _mapper.Map<List<PriceHistoryRepositoryModel>>(pricesHistory));
    }
}