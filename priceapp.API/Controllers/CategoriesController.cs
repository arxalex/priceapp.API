using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.API.Services.Interfaces;

namespace priceapp.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoriesService _categoriesService;

    public CategoriesController(ICategoriesService categoriesService)
    {
        _categoriesService = categoriesService;
    }

    [HttpGet("shop/{shopId:int}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> GetCategoryLinksByShopAsync([FromRoute] int shopId)
    {
        return Ok(await _categoriesService.GetCategoryLinksByShopAsync(shopId));
    }

    [HttpGet("")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> GetCategoriesAsync()
    {
        return Ok(await _categoriesService.GetCategoriesAsync());
    }
}