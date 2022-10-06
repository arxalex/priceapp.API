using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.Models;
using priceapp.Services.Interfaces;

namespace priceapp.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class CountriesController : ControllerBase
{
    private readonly ICountriesService _countriesService;

    public CountriesController(ICountriesService countriesService)
    {
        _countriesService = countriesService;
    }

    [HttpGet("")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> GetCountriesAsync()
    {
        return Ok(await _countriesService.GetCountriesAsync());
    }
    
    [HttpPost("")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> InsertCountryAsync([FromBody] CountryModel model)
    {
        await _countriesService.InsertCountryAsync(model);
        return Ok();
    }
    
    [HttpPost("{id:int}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> UpdateCountryAsync([FromBody] CountryModel model, [FromRoute] int id)
    {
        if (model.Id != id)
        {
            return BadRequest();
        }
        await _countriesService.UpdateCountryAsync(model);
        return Ok();
    }
}