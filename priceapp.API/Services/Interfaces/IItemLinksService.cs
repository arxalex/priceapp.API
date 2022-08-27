using priceapp.API.Models;

namespace priceapp.API.Services.Interfaces;

public interface IItemLinksService
{
    Task<List<ItemLinkModel>> GetItemLinksAsync(int shopId);
    Task InsertItemLinkAsync(ItemLinkModel model);
    Task<List<ItemLinkModel>> GetItemLinksAsync(int shopId, int categoryId);
}