using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.API.Services.Interfaces;

namespace priceapp.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class PackagesController : ControllerBase
{
    private readonly IPackagesService _packagesService;

    public PackagesController(IPackagesService packagesService)
    {
        _packagesService = packagesService;
    }

    [HttpGet("")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> GetPackagesAsync()
    {
        return Ok(await _packagesService.GetPackagesAsync());
    }
}