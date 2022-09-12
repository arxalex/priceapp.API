using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.API.Services.Interfaces;

namespace priceapp.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]

public class FilialsController : ControllerBase
{
    private readonly IFilialsService _filialsService;
    private readonly proxy.Controllers.FilialsController _filialsController;

    public FilialsController(IFilialsService filialsService, proxy.Controllers.FilialsController filialsController)
    {
        _filialsService = filialsService;
        _filialsController = filialsController;
    }

    [HttpPost("actualize")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> ActualizeFilialsAsync()
    {
        await _filialsService.ActualizeFilialsAsync();
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