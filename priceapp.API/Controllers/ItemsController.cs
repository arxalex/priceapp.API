using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.API.Controllers.Models.Request;
using priceapp.Models;
using priceapp.Services.Interfaces;
using priceapp.ShopsServices.Interfaces;

namespace priceapp.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IItemsService _itemsService;
    private readonly IItemLinksService _itemLinksService;
    private readonly proxy.Controllers.ItemsController _itemsController;
    private readonly ISilpoService _silpoService;
    private readonly IAtbService _atbService;
    private readonly IForaService _foraService;

    public ItemsController(IItemsService itemsService, IItemLinksService itemLinksService,
        proxy.Controllers.ItemsController itemsController, IForaService foraService, IAtbService atbService, ISilpoService silpoService)
    {
        _itemsService = itemsService;
        _itemLinksService = itemLinksService;
        _itemsController = itemsController;
        _foraService = foraService;
        _atbService = atbService;
        _silpoService = silpoService;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> GetItem([FromRoute] int id)
    {
        return Ok(await _itemsService.GetItemAsync(id));
    }

    [HttpGet("{id}/extended")]
    [Authorize(Roles = "1, 2, 3, 4, 5, 6, 7, 8, 9")]
    public async Task<IActionResult> GetItemExtended([FromRoute] int id)
    {
        return Ok(await _itemsService.GetItemExtendedAsync(id));
    }

    [HttpPost("{id}/location/extended")]
    [Authorize(Roles = "1, 2, 3, 4, 5, 6, 7, 8, 9")]
    public async Task<IActionResult> GetItemExtended([FromRoute] int id,
        [FromBody] LocationRequestModel model)
    {
        return Ok(await _itemsService.GetItemExtendedAsync(id, model.XCord, model.YCord, model.Radius));
    }

    [HttpGet("category/{categoryId:int}/extended")]
    [Authorize(Roles = "1, 2, 3, 4, 5, 6, 7, 8, 9")]
    public async Task<IActionResult> GetItemsExtended([FromRoute] int categoryId,
        [FromQuery] int from,
        [FromQuery] int to)
    {
        return Ok(await _itemsService.GetItemsExtendedAsync(categoryId, from, to));
    }

    [HttpPost("category/{categoryId:int}/location/extended")]
    [Authorize(Roles = "1, 2, 3, 4, 5, 6, 7, 8, 9")]
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
    [Authorize(Roles = "1, 2, 3, 4, 5, 6, 7, 8, 9")]
    public async Task<IActionResult> SearchItemsExtended(
        [FromQuery] int from,
        [FromQuery] int to,
        [FromBody] SearchRequestModel model)
    {
        return Ok(await _itemsService.SearchItemsExtendedAsync(model.Search, from, to));
    }

    [HttpPost("search/location/extended")]
    [Authorize(Roles = "1, 2, 3, 4, 5, 6, 7, 8, 9")]
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
        var shopItems = shopId switch
        {
            1 => await _silpoService.GetItemsByCategoryAsync(internalCategoryId, from, to),
            2 => await _foraService.GetItemsByCategoryAsync(internalCategoryId, from, to),
            3 => await _atbService.GetItemsByCategoryAsync(internalCategoryId, from, to),
            _ => new List<ItemShopModel>()
        };
        return Ok(shopItems);
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

    [HttpPost("actualize/proxy/{shopId:int}")]
    [Authorize(Roles = "9")]
    public async Task<IActionResult> ActualizeProxyItemsAsync([FromRoute] int shopId)
    {
        await _itemsController.ActualizeItems(shopId);
        return Ok();
    }
}