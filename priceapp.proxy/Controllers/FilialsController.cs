using priceapp.proxy.Models;
using priceapp.proxy.Services.Interfaces;
using priceapp.proxy.ShopServices.Interfaces;

namespace priceapp.proxy.Controllers;

public class FilialsController
{
    private readonly IFilialsService _filialsService;
    private readonly IAtbService _atbService;

    public FilialsController(IFilialsService filialsService, IAtbService atbService)
    {
        _filialsService = filialsService;
        _atbService = atbService;
    }

    public async Task<List<AtbFilialModel>> GetAtbFilialsAsync()
    {
        return await _filialsService.GetAtbFilialsAsync();
    }

    public async Task ActualizeFilialsAsync(int shopId)
    {
        if (shopId == 3)
        {
            var filials = await _atbService.GetFilialsAsync();
            await _filialsService.InsertAsync(filials);
        }
    }
}