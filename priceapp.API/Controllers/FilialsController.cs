using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.Models;
using priceapp.Services.Interfaces;
using priceapp.ShopsServices.Interfaces;

namespace priceapp.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class FilialsController : ControllerBase
{
    private readonly IFilialsService _filialsService;
    private readonly proxy.Controllers.FilialsController _filialsController;
    private readonly ISilpoService _silpoService;
    private readonly IForaService _foraService;
    private readonly IAtbService _atbService;
    private readonly IShopsService _shopsService;

    public FilialsController(IFilialsService filialsService, proxy.Controllers.FilialsController filialsController, ISilpoService silpoService, IForaService foraService, IAtbService atbService, IShopsService shopsService)
    {
        _filialsService = filialsService;
        _filialsController = filialsController;
        _silpoService = silpoService;
        _foraService = foraService;
        _atbService = atbService;
        _shopsService = shopsService;
    }

    [HttpPost("actualize")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> ActualizeFilialsAsync()
    {
        var shops = await _shopsService.GetShopsAsync();
        var filials = new List<FilialModel>();
        var filialsInserted = await _filialsService.GetFilialsAsync();
        foreach (var shop in shops)
        {
            var filialsFromShop = shop.Id switch
            {
                1 => await _silpoService.GetFilialsAsync(),
                2 => await _foraService.GetFilialsAsync(),
                3 => await _atbService.GetFilialsAsync(),
                _ => new List<FilialModel>()
            };
            filials.AddRange(filialsFromShop);
        }

        var filialsToInsert = filials
            .Where(x => filialsInserted.Count(y => y.InShopId == x.InShopId) < 1)
            .ToList();

        await _filialsService.InsertFilialsAsync(filialsToInsert);

        return Ok();
    }

    [HttpPost("actualize/proxy/{shopId:int}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> ActualizeProxyFilialsAsync([FromRoute] int shopId)
    {
        await _filialsController.ActualizeFilialsAsync(shopId);
        return Ok();
    }
}