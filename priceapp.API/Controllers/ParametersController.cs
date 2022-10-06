using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.API.Controllers.Models.Response;

namespace priceapp.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ParametersController : ControllerBase
{
    private readonly SessionParameters _sessionParameters;

    public ParametersController(SessionParameters sessionParameters)
    {
        _sessionParameters = sessionParameters;
    }

    [HttpGet("status")]
    [Authorize(Roles = "9")]
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