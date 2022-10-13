using Microsoft.AspNetCore.Mvc;
using priceapp.ControllersLogic;

namespace priceapp.Background.Controllers;

[ApiController]
[Route("[controller]")]
public class InfoController : ControllerBase
{
    private readonly SessionParameters _sessionParameters;

    public InfoController(SessionParameters sessionParameters)
    {
        _sessionParameters = sessionParameters;
    }

    [HttpGet("")]
    public Task<IActionResult> GetVersion()
    {
        return Task.FromResult<IActionResult>(Ok(new
        {
            Name = "Priceapp.Background",
            Version = "1.1"
        }));
    }

    [HttpGet("status/actualize")]
    public Task<IActionResult> GetActualizeStatus()
    {
        return Task.FromResult<IActionResult>(Ok(_sessionParameters.IsActualizePricesActive));
    }
}