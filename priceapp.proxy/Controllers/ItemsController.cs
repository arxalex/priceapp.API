using priceapp.proxy.Services.Interfaces;
using priceapp.proxy.Services.Models;
using priceapp.proxy.ShopServices.Interfaces;

namespace priceapp.proxy.Controllers;

public class ItemsController
{
    private readonly IItemsService _itemsService;
    private readonly IAtbService _atbService;
    private readonly ICategoriesService _categoriesService;

    public ItemsController(IItemsService itemsService, IAtbService atbService, ICategoriesService categoriesService)
    {
        _itemsService = itemsService;
        _atbService = atbService;
        _categoriesService = categoriesService;
    }

    public async Task<List<AtbItemModel>> GetAtbItemsAsync(int categoryId, int from, int to)
    {
        return await _itemsService.GetAtbItemsAsync(categoryId, from, to);
    }

    public async Task ActualizeItems(int shopId)
    {
        if (shopId == 3)
        {
            var items = new List<AtbItemModel>();
            var categories = await _categoriesService.GetAtbCategoriesAsync();

            foreach (var category in categories)
            {
                items.AddRange(await _atbService.GetItemsAsync(category.InternalId));
            }

            await _itemsService.InsertAsync(items);
        }
    }
}