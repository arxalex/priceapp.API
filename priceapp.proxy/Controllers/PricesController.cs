using priceapp.proxy.Services.Interfaces;
using priceapp.proxy.Services.Models;
using priceapp.proxy.ShopServices.Interfaces;
using priceapp.proxy.Utils;
using priceapp.tasks;

namespace priceapp.proxy.Controllers;

public class PricesController
{
    private readonly IFilialsService _filialsService;
    private readonly IAtbService _atbService;
    private readonly IPricesService _pricesService;
    private readonly ICategoriesService _categoriesService;
    private readonly ThreadsUtil _threadsUtil;
    private readonly SessionParameters _sessionParameters;

    public PricesController(IFilialsService filialsService, IAtbService atbService, IPricesService pricesService,
        ICategoriesService categoriesService, ThreadsUtil threadsUtil, SessionParameters sessionParameters)
    {
        _filialsService = filialsService;
        _atbService = atbService;
        _pricesService = pricesService;
        _categoriesService = categoriesService;
        _threadsUtil = threadsUtil;
        _sessionParameters = sessionParameters;
    }

    public async Task<List<PriceModel>> GetPricesAsync(int categoryId, int shopId, int filialId)
    {
        return await _pricesService.GetPricesAsync(categoryId, shopId, filialId);
    }

    public async Task StartActualizePricesTasksAsync(int shopId)
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
                        catch (Exception)
                        {
                            // ignored
                        }
                    }

                    await _threadsUtil.AddTask(Action());
                }
            }

            await _threadsUtil.AddTask(new Task(() => _sessionParameters.IsActualizeProxyAtbPricesActive = false));
        }
    }

    public async Task ActualizePricesAsync(int shopId)
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
                    catch (Exception)
                    {
                        _sessionParameters.IsActualizeProxyAtbPricesActive = false;
                        throw;
                    }
                }
            }

            _sessionParameters.IsActualizeProxyAtbPricesActive = false;
        }
    }
}