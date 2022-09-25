using AutoMapper;
using priceapp.API.Models;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Repositories.Models;
using priceapp.API.Services.Interfaces;
using priceapp.API.ShopServices.Interfaces;
using priceapp.API.Utils;
using priceapp.tasks;

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
    private readonly ThreadsUtil _threadsUtil;
    private readonly SessionParameters _sessionParameters;

    public PricesService(ISilpoService silpoService, IForaService foraService, IAtbService atbService,
        IPricesRepository pricesRepository, IMapper mapper, IFilialsService filialsService,
        ICategoriesService categoriesService, ThreadsUtil threadsUtil, SessionParameters sessionParameters)
    {
        _silpoService = silpoService;
        _foraService = foraService;
        _atbService = atbService;
        _pricesRepository = pricesRepository;
        _mapper = mapper;
        _filialsService = filialsService;
        _categoriesService = categoriesService;
        _threadsUtil = threadsUtil;
        _sessionParameters = sessionParameters;
    }

    public async Task<List<PriceModel>> GetPricesAsync(int shopId, int internalFilialId, int categoryId)
    {
        var prices = shopId switch
        {
            1 => await _silpoService.GetPricesAsync(categoryId, internalFilialId),
            2 => await _foraService.GetPricesAsync(categoryId, internalFilialId),
            3 => await _atbService.GetPricesAsync(categoryId, internalFilialId),
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
            prices.AddRange(await GetPricesAsync(filial.ShopId, filial.InShopId, category.Id));
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

        await _pricesRepository.InsertOrUpdatePricesAsync(_mapper.Map<List<PriceRepositoryModel>>(prices));
        await _pricesRepository.InsertOrUpdatePricesHistoryAsync(
            _mapper.Map<List<PriceHistoryRepositoryModel>>(pricesHistory));
    }

    public async Task SetPriceQuantitiesZeroAsync()
    {
        await _pricesRepository.SetPriceQuantitiesZeroAsync();
    }

    public async Task SetPriceQuantitiesZeroAsync(int filialId)
    {
        await _pricesRepository.SetPriceQuantitiesZeroAsync(filialId);
    }

    public async Task StartUpdatePricesTasksAsync(bool forceUpdate = false, bool skipSetZeroQuantity = false)
    {
        if (_sessionParameters.IsActualizePricesActive)
        {
            return;
        }

        _sessionParameters.IsActualizePricesActive = true;

        var lastFilial = forceUpdate ? 0 : await _pricesRepository.GetMaxFilialIdToday();
        var filials = forceUpdate
            ? await _filialsService.GetFilialsAsync()
            : (await _filialsService.GetFilialsAsync()).Where(x => x.Id >= lastFilial).ToList();

        foreach (var filial in filials)
        {
            async Task Action()
            {
                try
                {
                    if (!skipSetZeroQuantity)
                    {
                        await SetPriceQuantitiesZeroAsync(filial.Id);
                    }

                    await UpdatePricesAsync(filial);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            await _threadsUtil.AddTask(Action(), Priority.Medium);
        }

        await _threadsUtil.AddTask(new Task(() => _sessionParameters.IsActualizePricesActive = false), Priority.Medium);
    }

    public async Task UpdatePricesAsync(bool forceUpdate = false, bool skipSetZeroQuantity = false)
    {
        if (_sessionParameters.IsActualizePricesActive)
        {
            return;
        }

        _sessionParameters.IsActualizePricesActive = true;

        var lastFilial = forceUpdate ? 0 : await _pricesRepository.GetMaxFilialIdToday();
        var filials = forceUpdate
            ? await _filialsService.GetFilialsAsync()
            : (await _filialsService.GetFilialsAsync()).Where(x => x.Id >= lastFilial).ToList();

        foreach (var filial in filials)
        {
            try
            {
                if (!skipSetZeroQuantity)
                {
                    await SetPriceQuantitiesZeroAsync(filial.Id);
                }

                await UpdatePricesAsync(filial);
            }
            catch (Exception)
            {
                _sessionParameters.IsActualizePricesActive = false;
                throw;
            }
        }

        _sessionParameters.IsActualizePricesActive = false;
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
}