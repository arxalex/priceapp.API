using priceapp.Models;
using priceapp.Models.Enums;

namespace priceapp.Services.Interfaces;

public interface IShoppingListService
{
    Task<(List<PriceModel>, double)> ProcessShoppingList(CartProcessingType type, List<ShoppingListModel> items,
        double xCord, double yCord, double radius);
}