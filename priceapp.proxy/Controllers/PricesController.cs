using priceapp.proxy.Services.Interfaces;
using priceapp.proxy.Services.Models;
using priceapp.proxy.ShopServices.Interfaces;

namespace priceapp.proxy.Controllers;

public class PricesController
{
    private readonly IFilialsService _filialsService;
    private readonly IAtbService _atbService;
    private readonly IPricesService _pricesService;
    private readonly ICategoriesService _categoriesService;

    public PricesController(IFilialsService filialsService, IAtbService atbService, IPricesService pricesService, ICategoriesService categoriesService)
    {
        _filialsService = filialsService;
        _atbService = atbService;
        _pricesService = pricesService;
        _categoriesService = categoriesService;
    }

    public async Task<List<PriceModel>> GetPricesAsync(int categoryId, int shopId, int filialId)
    {
        return await _pricesService.GetPricesAsync(categoryId, shopId, filialId);
    }

    public async Task ActualizePricesAsync(int shopId)
    {
        if (shopId == 3)
        {
            var prices = new List<PriceModel>();
            var filials = await _filialsService.GetAtbFilialsAsync();
            var categories = await _categoriesService.GetAtbCategoriesAsync();
            foreach (var filial in filials)
            {
                foreach (var category in categories)
                {
                    try
                    {
                        prices.AddRange(await _atbService.GetPricesAsync(category.Id, filial.Id));
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }

            await _pricesService.InsertAsync(prices);
        }
    }
}