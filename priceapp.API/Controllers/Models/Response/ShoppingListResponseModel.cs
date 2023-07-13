using priceapp.Models;

namespace priceapp.API.Controllers.Models.Response;

public class ShoppingListResponseModel
{
    public List<PriceModel> ShoppingList { get; set; } = new();
    public double Economy { get; set; }
    public List<int> ItemIdsNotFound { get; set; } = new();
}