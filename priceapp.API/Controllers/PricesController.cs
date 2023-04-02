using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.API.Controllers.Models.Request;
using priceapp.ControllersLogic;
using priceapp.Services.Interfaces;

namespace priceapp.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class PricesController : ControllerBase
{
    private readonly IPricesService _pricesService;
    private readonly PricesControllerUpdateLogic _pricesControllerUpdateLogic;

    public PricesController(IPricesService pricesService,  PricesControllerUpdateLogic pricesControllerUpdateLogic)
    {
        _pricesService = pricesService;
        _pricesControllerUpdateLogic = pricesControllerUpdateLogic;
    }

    [HttpPost("actualize")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> ActualizePricesAsync()
    {
        await _pricesControllerUpdateLogic.ActualizePricesAsync();
        return Ok();
    }

    [HttpPost("actualize/proxy/{shopId:int}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> ActualizeProxyPricesAsync([FromRoute] int shopId)
    {
        await _pricesControllerUpdateLogic.ActualizeProxyPricesAsync(shopId);
        return Ok();
    }

    [HttpPost("refactor")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> RefactorPricesAsync()
    {
        await _pricesService.RefactorPricesAsync();
        return Ok();
    }
    
    [HttpPost("{itemId:int}/location/")]
    [Authorize(Roles = "1, 2, 3, 4, 5, 6, 7, 8, 9")]
    public async Task<IActionResult> GetPricesAndFilialsAsync([FromRoute] int itemId,
        [FromBody] LocationRequestModel model)
    {
        return Ok(await _pricesService.GetPricesAsync(itemId, model.XCord, model.YCord, model.Radius));
    }
}