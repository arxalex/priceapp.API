using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.API.Controllers.Models.Request;
using priceapp.API.Controllers.Models.Response;
using priceapp.Models.Enums;
using priceapp.Services.Interfaces;

namespace priceapp.API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]

public class ShoppingListController : ControllerBase
{
    private readonly IShoppingListService _shoppingListService;

    public ShoppingListController(IShoppingListService shoppingListService)
    {
        _shoppingListService = shoppingListService;
    }

    [HttpPost("location")]
    [Authorize(Roles = "0, 1, 2, 3, 4, 5, 6, 7, 8, 9")]
    public async Task<IActionResult> ProcessShoppingList([FromQuery] CartProcessingType method,
        [FromBody] LocationAndItemsRequestModel model)
    {
        var (prices, economy, notfound) =
            await _shoppingListService.ProcessShoppingList(method, model.Items, model.XCord, model.YCord, model.Radius);
        var result = new ShoppingListResponseModel()
        {
            ShoppingList = prices,
            Economy = economy,
            ItemIdsNotFound = notfound
        };
        return Ok(result);
    }
}