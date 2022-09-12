using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.API.Models;
using priceapp.API.Services.Interfaces;

namespace priceapp.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoriesService _categoriesService;
    private readonly ICategoryLinksService _categoryLinksService;
    private readonly proxy.Controllers.CategoriesController _categoriesController;

    public CategoriesController(ICategoriesService categoriesService, ICategoryLinksService categoryLinksService, proxy.Controllers.CategoriesController categoriesController)
    {
        _categoriesService = categoriesService;
        _categoryLinksService = categoryLinksService;
        _categoriesController = categoriesController;
    }

    [HttpGet("shop/{shopId:int}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> GetCategoryLinksByShopAsync([FromRoute] int shopId)
    {
        return Ok(await _categoryLinksService.GetCategoryLinksAsync(shopId));
    }

    [HttpGet("")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> GetCategoriesAsync()
    {
        return Ok(await _categoriesService.GetCategoriesAsync());
    }
    
    [HttpPost("")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> InsertCategoryAsync([FromBody] CategoryModel model)
    {
        await _categoriesService.InsertCategoryAsync(model);
        return Ok();
    }
    
    [HttpPost("link/")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> InsertCategoryLinkAsync([FromBody] CategoryLinkModel model)
    {
        await _categoryLinksService.InsertCategoryLinkAsync(model);
        return Ok();
    }
    
    [HttpPost("{id:int}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> UpdateCategoryAsync([FromBody] CategoryModel model, [FromRoute] int id)
    {
        if (model.Id != id)
        {
            return BadRequest();
        }
        await _categoriesService.UpdateCategoryAsync(model);
        return Ok();
    }
    
    [HttpPost("link/{id:int}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> UpdateCategoryLinkAsync([FromBody] CategoryLinkModel model, [FromRoute] int id)
    {
        if (model.Id != id)
        {
            return BadRequest();
        }
        await _categoryLinksService.UpdateCategoryLinkAsync(model);
        return Ok();
    }
    
    [HttpPost("link/actualize")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> ActualizeCategoryLinksAsync()
    {
        await _categoryLinksService.ActualizeCategoryLinksAsync();
        return Ok();
    }
    
    [HttpPost("actualize/proxy/{shopId:int}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> ActualizeProxyCategoriesAsync([FromRoute] int shopId)
    {
        await _categoriesController.ActualizeCategoriesAsync(shopId);
        return Ok();
    }
}