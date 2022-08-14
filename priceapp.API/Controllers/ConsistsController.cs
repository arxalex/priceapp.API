using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.API.Services.Interfaces;

namespace priceapp.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ConsistsController : ControllerBase
{
    private readonly IConsistsService _consistsService;

    public ConsistsController(IConsistsService consistsService)
    {
        _consistsService = consistsService;
    }

    [HttpGet("")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> GetConsistsAsync()
    {
        return Ok(await _consistsService.GetConsistsAsync());
    }
}