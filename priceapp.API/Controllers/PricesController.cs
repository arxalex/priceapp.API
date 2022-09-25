using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.API.Services.Interfaces;

namespace priceapp.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class PricesController : ControllerBase
{
    private readonly IPricesService _pricesService;
    private readonly proxy.Controllers.PricesController _pricesController;

    public PricesController(IPricesService pricesService, proxy.Controllers.PricesController pricesController)
    {
        _pricesService = pricesService;
        _pricesController = pricesController;
    }

    [HttpPost("actualize")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> ActualizePricesAsync()
    {
        await _pricesService.StartUpdatePricesTasksAsync();
        return Ok();
    }

    [HttpPost("actualize/proxy/{shopId:int}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> ActualizeProxyPricesAsync([FromRoute] int shopId)
    {
        await _pricesController.StartActualizePricesTasksAsync(shopId);
        return Ok();
    }

    [HttpPost("refactor")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> RefactorPricesAsync()
    {
        await _pricesService.RefactorPricesAsync();
        return Ok();
    }
}