using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.API.Controllers.Models.Response;
using priceapp.ControllersLogic;

namespace priceapp.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ParametersController : ControllerBase
{
    private readonly SessionParameters _sessionParameters;

    public ParametersController(SessionParameters sessionParameters)
    {
        _sessionParameters = sessionParameters;
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        var status = new List<StatusResponseModel>
        {
            new()
            {
                Key = "IsActualizePricesActive",
                Value = _sessionParameters.IsActualizePricesActive
            },
            new()
            {
                Key = "IsActualizeProxyAtbPricesActive",
                Value = _sessionParameters.IsActualizeProxyAtbPricesActive
            }
        };
        return Ok(status);
    }
}