using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using priceapp.API.Controllers.Models.Request;
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
    [Authorize(Roles = "1")]
    public async Task<IActionResult> ProcessShoppingList([FromQuery] CartProcessingType method,
        [FromBody] LocationAndItemsRequestModel model)
    { 
        return Ok(await _shoppingListService.ProcessShoppingList(method, model.Items, model.XCord, model.YCord, model.Radius));
    }
}