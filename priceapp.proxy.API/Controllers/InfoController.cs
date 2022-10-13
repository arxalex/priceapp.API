using Microsoft.AspNetCore.Mvc;

namespace priceapp.proxy.API.Controllers;

[ApiController]
[Route("[controller]")]
public class InfoController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly SessionParameters _sessionParameters;

    public InfoController(IConfiguration configuration, SessionParameters sessionParameters)
    {
        _configuration = configuration;
        _sessionParameters = sessionParameters;
    }

    [HttpGet("")]
    public Task<IActionResult> GetVersion()
    {
        return Task.FromResult<IActionResult>(Ok(new
        {
            Name = "Priceapp.proxy.API",
            Version = "1.1"
        }));
    }

    [HttpGet("cores")]
    public Task<IActionResult> GetCores()
    {
        return Task.FromResult<IActionResult>(Ok(new
        {
            Count = bool.Parse(_configuration["Threads:UseSystem"])
                ? Environment.ProcessorCount
                : int.Parse(_configuration["Threads:DefaultCount"]),
            UseSystem = bool.Parse(_configuration["Threads:UseSystem"]),
            SystemCores = Environment.ProcessorCount
        }));
    }
    
    [HttpGet("status/actualize/proxy")]
    public Task<IActionResult> GetActualizeProxyStatus()
    {
        return Task.FromResult<IActionResult>(Ok(_sessionParameters.IsActualizeProxyAtbPricesActive));
    }
}