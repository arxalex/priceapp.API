using Microsoft.AspNetCore.Mvc;

namespace priceapp.proxy.API.Controllers;

[ApiController]
[Route("[controller]")]
public class InfoController : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> GetVersion()
    {
        return Ok(new
        {
            Name = "Priceapp.API",
            Version = "1.1"
        });
    }
}