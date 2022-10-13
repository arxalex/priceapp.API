using priceapp.proxy.Controllers.Logic;
using priceapp.proxy.Models;
using priceapp.proxy.Services.Interfaces;

namespace priceapp.proxy.Controllers;

public class PricesController
{
    private readonly IPricesService _pricesService;
    private readonly PricesControllerLogic _pricesControllerLogic;

    public PricesController(IPricesService pricesService, PricesControllerLogic pricesControllerLogic)
    {
        _pricesService = pricesService;
        _pricesControllerLogic = pricesControllerLogic;
    }

    public async Task<List<PriceModel>> GetPricesAsync(int categoryId, int shopId, int filialId)
    {
        return await _pricesService.GetPricesAsync(categoryId, shopId, filialId);
    }

    public async Task ActualizeProxyPricesAsync(int shopId)
    {
        await _pricesControllerLogic.ActualizeProxyPricesAsync(shopId);
    }
}