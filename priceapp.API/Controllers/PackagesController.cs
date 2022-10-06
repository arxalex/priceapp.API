using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.Models;
using priceapp.Services.Interfaces;

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
    
    [HttpPost("")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> InsertPackageAsync([FromBody] PackageModel model)
    {
        await _packagesService.InsertPackageAsync(model);
        return Ok();
    }
    
    [HttpPost("{id:int}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> UpdatePackageAsync([FromBody] PackageModel model, [FromRoute] int id)
    {
        if (model.Id != id)
        {
            return BadRequest();
        }
        await _packagesService.UpdatePackageAsync(model);
        return Ok();
    }
}