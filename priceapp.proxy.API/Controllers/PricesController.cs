using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace priceapp.proxy.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class PricesController : ControllerBase
{
    private readonly proxy.Controllers.PricesController _pricesController;

    public PricesController(proxy.Controllers.PricesController pricesController)
    {
        _pricesController = pricesController;
    }
    
    [HttpPost("actualize/proxy/{shopId:int}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> ActualizeProxyPricesAsync([FromRoute] int shopId)
    {
        await _pricesController.ActualizeProxyPricesAsync(shopId);
        return Ok();
    }
}