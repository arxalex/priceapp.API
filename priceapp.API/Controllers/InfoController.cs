using Microsoft.AspNetCore.Mvc;

namespace priceapp.API.Controllers;

[ApiController]
[Route("[controller]")]
public class InfoController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public InfoController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetVersion()
    {
        return Ok(new
        {
            Name = "Priceapp.API",
            Version = "1.1"
        });
    }

    [HttpGet("cores")]
    public async Task<IActionResult> GetCores()
    {
        return Ok(new
        {
            Count = bool.Parse(_configuration["Threads:UseSystem"])
                ? Environment.ProcessorCount
                : int.Parse(_configuration["Threads:DefaultCount"]),
            UseSystem = bool.Parse(_configuration["Threads:UseSystem"]),
            SystemCores = Environment.ProcessorCount
        });
    }
}