using priceapp.API.Models;

namespace priceapp.API.Services.Interfaces;

public interface IItemsService
{
    Task<List<ItemModel>> SearchItemsAsync(string search, int from, int to);
    Task<ItemModel> GetItemByIdAsync(int id);
    Task<List<ItemExtendedModel>> GetItemsExtendedByCategoryAsync(int categoryId, int from, int to);

    Task<List<ItemExtendedModel>> GetItemsExtendedByCategoryAndLocationAsync(int categoryId, double xCord, double yCord,
        double radius, int from, int to);

    Task<List<ItemExtendedModel>> SearchItemsExtendedAsync(string search, int from, int to);

    Task<List<ItemExtendedModel>> SearchItemsExtendedByLocationAsync(string search, double xCord, double yCord,
        double radius, int from, int to);

    Task<List<ItemModel>> SearchItemsByCategoryAsync(string search, int categoryId, int from, int to);
    Task<List<ItemModel>> GetItemsByShopAndCategoryAsync(int shopId, int categoryId, int from, int to);
    Task<ItemExtendedModel> GetItemExtendedByIdAsync(int id);
    Task<ItemExtendedModel> GetItemExtendedByIdAndLocationAsync(int id, double xCord, double yCord, double radius);
    Task<List<List<ItemModel>>> SearchMultipleItemsAsync(List<string> searchList, int from, int to);
    Task<List<ItemLinkModel>> GetItemLinksAsync(int shopId);
}