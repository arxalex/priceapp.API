using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.API.Controllers.Models.Request;
using priceapp.API.Services.Interfaces;

namespace priceapp.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IItemsService _itemsService;

    public ItemsController(IItemsService itemsService)
    {
        _itemsService = itemsService;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> GetItem([FromRoute] int id)
    {
        return Ok(await _itemsService.GetItemByIdAsync(id));
    }

    [HttpGet("{id}/extended")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> GetItemExtended([FromRoute] int id)
    {
        return Ok(await _itemsService.GetItemExtendedByIdAsync(id));
    }

    [HttpPost("{id}/location/extended")]
    public async Task<IActionResult> GetItemExtendedByLocation([FromRoute] int id,
        [FromBody] LocationRequestModel model)
    {
        return Ok(await _itemsService.GetItemExtendedByIdAndLocationAsync(id, model.XCord, model.YCord, model.Radius));
    }

    [HttpGet("category/{categoryId:int}/extended")]
    public async Task<IActionResult> GetItemsExtendedByCategory([FromRoute] int categoryId,
        [FromQuery] int from,
        [FromQuery] int to)
    {
        return Ok(await _itemsService.GetItemsExtendedByCategoryAsync(categoryId, from, to));
    }

    [HttpPost("category/{categoryId:int}/location/extended")]
    public async Task<IActionResult> GetItemsExtendedByCategoryAndLocation([FromRoute] int categoryId,
        [FromQuery] int from,
        [FromQuery] int to,
        [FromBody] LocationRequestModel model)
    {
        return Ok(await _itemsService.GetItemsExtendedByCategoryAndLocationAsync(categoryId, model.XCord, model.YCord,
            model.Radius, from, to));
    }

    [HttpPost("category/{categoryId:int}/search")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> SearchItemsByCategory(
        [FromQuery] int from,
        [FromQuery] int to,
        [FromRoute] int categoryId,
        [FromBody] SearchRequestModel model)
    {
        return Ok(await _itemsService.SearchItemsByCategoryAsync(model.Search, categoryId, from, to));
    }

    [HttpPost("search/extended")]
    public async Task<IActionResult> SearchItemsExtended(
        [FromQuery] int from,
        [FromQuery] int to,
        [FromBody] SearchRequestModel model)
    {
        return Ok(await _itemsService.SearchItemsExtendedAsync(model.Search, from, to));
    }

    [HttpPost("search/location/extended")]
    public async Task<IActionResult> SearchItemsExtendedByLocation(
        [FromQuery] int from,
        [FromQuery] int to,
        [FromBody] SearchAndLocationRequestModel model)
    {
        return Ok(await _itemsService.SearchItemsExtendedByLocationAsync(model.Search, model.XCord, model.YCord,
            model.Radius, from, to));
    }

    [HttpPost("search")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> SearchItems(
        [FromQuery] int from,
        [FromQuery] int to,
        [FromBody] SearchRequestModel model)
    {
        return Ok(await _itemsService.SearchItemsAsync(model.Search, from, to));
    }

    [HttpGet("shop/{shopId:int}/category/{categoryId:int}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> GetItemsByShop([FromRoute] int shopId,
        [FromRoute] int categoryId,
        [FromQuery] int from,
        [FromQuery] int to
    )
    {
        return Ok(await _itemsService.GetItemsByShopAndCategoryAsync(shopId, categoryId, from, to));
    }
}