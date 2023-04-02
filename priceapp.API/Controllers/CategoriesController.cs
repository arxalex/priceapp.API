using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.Models;
using priceapp.Services.Interfaces;
using priceapp.ShopsServices.Interfaces;

namespace priceapp.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoriesService _categoriesService;
    private readonly ICategoryLinksService _categoryLinksService;
    private readonly proxy.Controllers.CategoriesController _categoriesController;
    private readonly ISilpoService _silpoService;
    private readonly IForaService _foraService;
    private readonly IAtbService _atbService;
    private readonly IShopsService _shopsService;

    public CategoriesController(ICategoriesService categoriesService, ICategoryLinksService categoryLinksService,
        proxy.Controllers.CategoriesController categoriesController, ISilpoService silpoService, IForaService foraService, IAtbService atbService, IShopsService shopsService)
    {
        _categoriesService = categoriesService;
        _categoryLinksService = categoryLinksService;
        _categoriesController = categoriesController;
        _silpoService = silpoService;
        _foraService = foraService;
        _atbService = atbService;
        _shopsService = shopsService;
    }

    [HttpGet("shop/{shopId:int}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> GetCategoryLinksByShopAsync([FromRoute] int shopId)
    {
        return Ok(await _categoryLinksService.GetCategoryLinksAsync(shopId));
    }

    [HttpGet("{id:int}/child")]
    [Authorize(Roles = "1, 2, 3, 4, 5, 6, 7, 8, 9")]
    public async Task<IActionResult> GetChildCategoriesAsync([FromRoute] int id)
    {
        return Ok(await _categoriesService.GetChildLevelCategoriesAsync(id));
    }
    
    [HttpGet("base")]
    [Authorize(Roles = "1, 2, 3, 4, 5, 6, 7, 8, 9")]
    public async Task<IActionResult> GetBaseCategoriesAsync()
    {
        return Ok(await _categoriesService.GetBaseCategoriesAsync());
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
        var shops = await _shopsService.GetShopsAsync();
        var links = new List<CategoryLinkModel>();
        foreach (var shop in shops)
        {
            var categories = shop.Id switch
            {
                1 => await _silpoService.GetCategoryLinksAsync(),
                2 => await _foraService.GetCategoryLinksAsync(),
                3 => await _atbService.GetCategoryLinksAsync(),
                _ => new List<CategoryLinkModel>()
            };
            links.AddRange(categories);
        }

        await _categoryLinksService.InsertOrUpdateCategoryLinksAsync(links);
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