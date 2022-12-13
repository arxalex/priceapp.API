using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using priceapp.Models;
using priceapp.Services.Interfaces;
using priceapp.ShopsServices.Interfaces;
using priceapp.tasks;

namespace priceapp.ControllersLogic;

public class PricesControllerUpdateLogic
{
    private readonly ISilpoService _silpoService;
    private readonly IForaService _foraService;
    private readonly IAtbService _atbService;
    private readonly ICategoriesService _categoriesService;
    private readonly SessionParameters _sessionParameters;
    private readonly IFilialsService _filialsService;
    private readonly ThreadsUtil _threadsUtil;
    private readonly IConfiguration _configuration;
    private readonly IPricesService _pricesService;
    private readonly proxy.Controllers.PricesController _pricesController;
    private readonly ILogger<PricesControllerUpdateLogic> _logger;

    public PricesControllerUpdateLogic(ISilpoService silpoService, IForaService foraService, IAtbService atbService,
        ICategoriesService categoriesService, SessionParameters sessionParameters, IFilialsService filialsService,
        ThreadsUtil threadsUtil, IConfiguration configuration, IPricesService pricesService, proxy.Controllers.PricesController pricesController, ILogger<PricesControllerUpdateLogic> logger)
    {
        _silpoService = silpoService;
        _foraService = foraService;
        _atbService = atbService;
        _categoriesService = categoriesService;
        _sessionParameters = sessionParameters;
        _filialsService = filialsService;
        _threadsUtil = threadsUtil;
        _configuration = configuration;
        _pricesService = pricesService;
        _pricesController = pricesController;
        _logger = logger;
    }

    public async Task ActualizeProxyPricesAsync(int shopId)
    {
        if (bool.Parse(_configuration["Proxy:MultiInstance"]))
        {
            throw new Exception("Using multi-instance. Updating should work in background");
        }

        await _pricesController.ActualizeProxyPricesAsync(shopId);
    }

    public async Task ActualizePricesAsync()
    {
        if (bool.Parse(_configuration["Threads:UseMultiThreading"]))
        {
            await StartUpdatePricesTasksAsync();
            return;
        }

        await UpdatePricesAsync();
    }

    private async Task<List<PriceModel>> GetPricesAsync(int shopId, int internalFilialId, int filialId, int categoryId)
    {
        var prices = shopId switch
        {
            1 => await _silpoService.GetPricesAsync(categoryId, internalFilialId, filialId),
            2 => await _foraService.GetPricesAsync(categoryId, internalFilialId, filialId),
            3 => await _atbService.GetPricesAsync(categoryId, internalFilialId, filialId),
            _ => new List<PriceModel>()
        };

        return prices;
    }

    private async Task UpdatePricesAsync(FilialModel filial)
    {
        var categories = await _categoriesService.GetBaseCategoriesAsync();
        var prices = new List<PriceModel>();
        foreach (var category in categories)
        {
            prices.AddRange(await GetPricesAsync(filial.ShopId, filial.InShopId, filial.Id, category.Id));
        }

        var pricesHistory = prices.Select(x => new PriceHistoryModel()
        {
            Id = -1,
            Date = DateTime.Now,
            FilialId = x.FilialId,
            ItemId = x.ItemId,
            Price = x.Price,
            ShopId = x.ShopId
        }).ToList();

        await _pricesService.InsertOrUpdatePricesAsync(prices);
        await _pricesService.InsertOrUpdatePricesHistoryAsync(pricesHistory);
    }

    private async Task StartUpdatePricesTasksAsync(bool forceUpdate = false, bool skipSetZeroQuantity = false)
    {
        if (_sessionParameters.IsActualizePricesActive)
        {
            return;
        }

        _sessionParameters.IsActualizePricesActive = true;

        var lastFilial = forceUpdate ? 0 : await _pricesService.GetMaxFilialIdToday() ?? 0;
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
                        await _pricesService.SetPriceQuantitiesZeroAsync(filial.Id);
                    }

                    await UpdatePricesAsync(filial);
                }
                catch (Exception e)
                {
                    _logger.LogWarning("Process of actualizing prices for category {FilialId} throw exception: {EMessage}", filial.Id, e.Message);
                    _logger.LogInformation("Continue after error");
                }
            }

            await _threadsUtil.AddTask(Action(), Priority.Medium);
        }

        await _threadsUtil.AddTask(new Task(() => _sessionParameters.IsActualizePricesActive = false), Priority.Medium);
    }

    private async Task UpdatePricesAsync(bool forceUpdate = false, bool skipSetZeroQuantity = false)
    {
        if (_sessionParameters.IsActualizePricesActive)
        {
            return;
        }

        _sessionParameters.IsActualizePricesActive = true;

        var lastFilial = forceUpdate ? 0 : await _pricesService.GetMaxFilialIdToday() ?? 0;
        var filials = forceUpdate
            ? await _filialsService.GetFilialsAsync()
            : (await _filialsService.GetFilialsAsync()).Where(x => x.Id >= lastFilial).ToList();

        foreach (var filial in filials)
        {
            try
            {
                if (!skipSetZeroQuantity)
                {
                    await _pricesService.SetPriceQuantitiesZeroAsync(filial.Id);
                }

                await UpdatePricesAsync(filial);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Process of actualizing prices for filial {FilialId} throw exception: {EMessage}", filial.Id, e.Message);
                _logger.LogInformation("Continue after error");
            }
        }

        _sessionParameters.IsActualizePricesActive = false;
    }
}