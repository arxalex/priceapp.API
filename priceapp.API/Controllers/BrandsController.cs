using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.Models;
using priceapp.Services.Interfaces;

namespace priceapp.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class BrandsController : ControllerBase
{
    private readonly IBrandsService _brandsService;

    public BrandsController(IBrandsService brandsService)
    {
        _brandsService = brandsService;
    }

    [HttpGet("")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> GetBrandsAsync()
    {
        return Ok(await _brandsService.GetBrandsAsync());
    }
    
    [HttpPost("")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> InsertBrandAsync([FromBody] BrandModel model)
    {
        await _brandsService.InsertBrandAsync(model);
        return Ok();
    }
    
    [HttpPost("{id:int}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> UpdateBrandAsync([FromBody] BrandModel model, [FromRoute] int id)
    {
        if (model.Id != id)
        {
            return BadRequest();
        }
        await _brandsService.UpdateBrandAsync(model);
        return Ok();
    }

    [HttpGet("{id:int}/alerts")]
    [Authorize(Roles = "1, 2, 3, 4, 5, 6, 7, 8, 9")]
    public async Task<IActionResult> GetBrandAlertsAsync([FromRoute] int id)
    {
        return Ok(await _brandsService.GetBrandAlertsAsync(id));
    }
}