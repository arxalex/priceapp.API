using priceapp.API.Models;

namespace priceapp.API.Services.Interfaces;

public interface IItemsService
{
    Task<List<ItemModel>> SearchItemsAsync(string search, int from, int to);
    Task<ItemModel> GetItemAsync(int id);
    Task<List<ItemExtendedModel>> GetItemsExtendedAsync(int categoryId, int from, int to);

    Task<List<ItemExtendedModel>> GetItemsExtendedAsync(int categoryId, double xCord, double yCord,
        double radius, int from, int to);

    Task<List<ItemExtendedModel>> SearchItemsExtendedAsync(string search, int from, int to);

    Task<List<ItemExtendedModel>> SearchItemsExtendedAsync(string search, double xCord, double yCord,
        double radius, int from, int to);

    Task<List<ItemModel>> SearchItemsAsync(string search, int categoryId, int from, int to);
    Task<List<ItemShopModel>> GetShopItemsAsync(int shopId, int categoryId, int from, int to);
    Task<ItemExtendedModel> GetItemExtendedAsync(int id);
    Task<ItemExtendedModel> GetItemExtendedAsync(int id, double xCord, double yCord, double radius);
    Task<List<List<ItemModel>>> SearchMultipleItemsAsync(List<string> searchList, int from, int to);
    Task InsertItemAsync(ItemModel model);
    Task UpdateItemAsync(ItemModel model);
    Task InsertItemLinkAsync(ItemLinkModel model);
    Task<ItemModel> GetLastInsertedItemAsync();
}