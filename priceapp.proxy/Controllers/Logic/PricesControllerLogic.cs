using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using priceapp.proxy.Services.Interfaces;
using priceapp.proxy.ShopServices.Interfaces;
using priceapp.tasks;

namespace priceapp.proxy.Controllers.Logic;

public class PricesControllerLogic
{
    private readonly ICategoriesService _categoriesService;
    private readonly ThreadsUtil _threadsUtil;
    private readonly SessionParameters _sessionParameters;
    private readonly IFilialsService _filialsService;
    private readonly IAtbService _atbService;
    private readonly IPricesService _pricesService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PricesControllerLogic> _logger;

    public PricesControllerLogic(ICategoriesService categoriesService, ThreadsUtil threadsUtil, SessionParameters sessionParameters, IFilialsService filialsService, IAtbService atbService, IPricesService pricesService, IConfiguration configuration, ILogger<PricesControllerLogic> logger)
    {
        _categoriesService = categoriesService;
        _threadsUtil = threadsUtil;
        _sessionParameters = sessionParameters;
        _filialsService = filialsService;
        _atbService = atbService;
        _pricesService = pricesService;
        _configuration = configuration;
        _logger = logger;
    }
    
    public async Task ActualizeProxyPricesAsync(int shopId)
    {
        if (bool.Parse(_configuration["Threads:UseMultiThreading"]))
        {
            await StartActualizePricesTasksAsync(shopId);
            return;
        }

        await ActualizePricesAsync(shopId);
    }

    private async Task StartActualizePricesTasksAsync(int shopId)
    {
        if (shopId == 3)
        {
            if (_sessionParameters.IsActualizeProxyAtbPricesActive)
            {
                return;
            }

            _sessionParameters.IsActualizeProxyAtbPricesActive = true;

            var filials = await _filialsService.GetAtbFilialsAsync();
            var categories = await _categoriesService.GetAtbBaseCategoriesAsync();
            foreach (var filial in filials)
            {
                foreach (var category in categories)
                {
                    async Task Action()
                    {
                        try
                        {
                            var prices = await _atbService.GetPricesAsync(category.Id, filial.Id);
                            await _pricesService.InsertAsync(prices);
                        }
                        catch (Exception e)
                        {
                            _logger.LogWarning("Process of actualizing prices for category {CategoryId} throw exception: {EMessage}", category.Id, e.Message);
                            _logger.LogInformation("Continue after error");
                        }
                    }

                    await _threadsUtil.AddTask(Action());
                }
            }

            await _threadsUtil.AddTask(new Task(() => _sessionParameters.IsActualizeProxyAtbPricesActive = false));
        }
    }

    private async Task ActualizePricesAsync(int shopId)
    {
        if (shopId == 3)
        {
            if (_sessionParameters.IsActualizeProxyAtbPricesActive)
            {
                return;
            }

            _sessionParameters.IsActualizeProxyAtbPricesActive = true;

            var filials = await _filialsService.GetAtbFilialsAsync();
            var categories = await _categoriesService.GetAtbBaseCategoriesAsync();
            foreach (var filial in filials)
            {
                foreach (var category in categories)
                {
                    try
                    {
                        var prices = await _atbService.GetPricesAsync(category.Id, filial.Id);
                        await _pricesService.InsertAsync(prices);
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning("Process of actualizing prices for category {CategoryId} throw exception: {EMessage}", category.Id, e.Message);
                        _logger.LogInformation("Continue after error");
                    }
                }
            }

            _sessionParameters.IsActualizeProxyAtbPricesActive = false;
        }
    }
}