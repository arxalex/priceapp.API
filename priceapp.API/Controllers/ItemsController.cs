using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.API.Controllers.Models.Request;
using priceapp.API.Models;
using priceapp.API.Services.Interfaces;

namespace priceapp.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IItemsService _itemsService;
    private readonly IItemLinksService _itemLinksService;

    public ItemsController(IItemsService itemsService, IItemLinksService itemLinksService)
    {
        _itemsService = itemsService;
        _itemLinksService = itemLinksService;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> GetItem([FromRoute] int id)
    {
        return Ok(await _itemsService.GetItemAsync(id));
    }

    [HttpGet("{id}/extended")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> GetItemExtended([FromRoute] int id)
    {
        return Ok(await _itemsService.GetItemExtendedAsync(id));
    }

    [HttpPost("{id}/location/extended")]
    public async Task<IActionResult> GetItemExtended([FromRoute] int id,
        [FromBody] LocationRequestModel model)
    {
        return Ok(await _itemsService.GetItemExtendedAsync(id, model.XCord, model.YCord, model.Radius));
    }

    [HttpGet("category/{categoryId:int}/extended")]
    public async Task<IActionResult> GetItemsExtended([FromRoute] int categoryId,
        [FromQuery] int from,
        [FromQuery] int to)
    {
        return Ok(await _itemsService.GetItemsExtendedAsync(categoryId, from, to));
    }

    [HttpPost("category/{categoryId:int}/location/extended")]
    public async Task<IActionResult> GetItemsExtended([FromRoute] int categoryId,
        [FromQuery] int from,
        [FromQuery] int to,
        [FromBody] LocationRequestModel model)
    {
        return Ok(await _itemsService.GetItemsExtendedAsync(categoryId, model.XCord, model.YCord,
            model.Radius, from, to));
    }

    [HttpPost("category/{categoryId:int}/search")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> SearchItems(
        [FromQuery] int from,
        [FromQuery] int to,
        [FromRoute] int categoryId,
        [FromBody] SearchRequestModel model)
    {
        return Ok(await _itemsService.SearchItemsAsync(model.Search, categoryId, from, to));
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
    public async Task<IActionResult> SearchItemsExtended(
        [FromQuery] int from,
        [FromQuery] int to,
        [FromBody] SearchAndLocationRequestModel model)
    {
        return Ok(await _itemsService.SearchItemsExtendedAsync(model.Search, model.XCord, model.YCord,
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

    [HttpPost("search/multiple")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> SearchMultipleItems(
        [FromQuery] int from,
        [FromQuery] int to,
        [FromBody] List<string> model)
    {
        return Ok(await _itemsService.SearchMultipleItemsAsync(model, from, to));
    }

    [HttpGet("shop/{shopId:int}/category/{internalCategoryId:int}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> GetItemsByShop([FromRoute] int shopId,
        [FromRoute] int internalCategoryId,
        [FromQuery] int from,
        [FromQuery] int to
    )
    {
        return Ok(await _itemsService.GetShopItemsAsync(shopId, internalCategoryId, from, to));
    }

    [HttpPost("")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> InsertItemAsync([FromBody] ItemModel model)
    {
        await _itemsService.InsertItemAsync(model);
        return Ok();
    }

    [HttpPost("{id:int}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> UpdateItemAsync([FromBody] ItemModel model, [FromRoute] int id)
    {
        if (model.Id != id)
        {
            return BadRequest();
        }

        await _itemsService.UpdateItemAsync(model);
        return Ok();
    }

    [HttpPost("link")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> InsertItemLinkAsync([FromBody] ItemLinkModel model)
    {
        await _itemLinksService.InsertItemLinkAsync(model);
        return Ok();
    }

    [HttpPost("last")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> GetLastInsertedItem([FromBody] SearchRequestModel model)
    {
        var item = await _itemsService.GetLastInsertedItemAsync();
        if (item.Label == model.Search)
        {
            return Ok(item);
        }

        return BadRequest();
    }
}