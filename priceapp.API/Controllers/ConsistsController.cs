using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.API.Models;
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
    
    [HttpPost("")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> InsertConsistAsync([FromBody] ConsistModel model)
    {
        await _consistsService.InsertConsistAsync(model);
        return Ok();
    }
    
    [HttpPost("{id:int}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> UpdateConsistAsync([FromBody] ConsistModel model, [FromRoute] int id)
    {
        if (model.Id != id)
        {
            return BadRequest();
        }
        await _consistsService.UpdateConsistAsync(model);
        return Ok();
    }
}