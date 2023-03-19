using System.ComponentModel.DataAnnotations;
using priceapp.Models;

namespace priceapp.API.Controllers.Models.Request;

public class LocationAndItemsRequestModel
{
    [Required] public double XCord { get; set; }
    [Required] public double YCord { get; set; }
    [Required] public double Radius { get; set; }
    [Required] public List<ShoppingListModel> Items { get; set; } = new();
}