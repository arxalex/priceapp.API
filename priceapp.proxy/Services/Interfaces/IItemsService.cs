using priceapp.proxy.Services.Models;

namespace priceapp.proxy.Services.Interfaces;

public interface IItemsService
{
    Task<List<AtbItemModel>> GetAtbItemsAsync();
    Task InsertAsync(AtbItemModel model);
    Task<List<AtbItemModel>> GetAtbItemsAsync(int categoryId, int from, int to);
    Task InsertAsync(List<AtbItemModel> models);
}