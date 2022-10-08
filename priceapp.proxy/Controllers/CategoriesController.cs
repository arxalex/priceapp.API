using priceapp.proxy.Models;
using priceapp.proxy.Services.Interfaces;
using priceapp.proxy.ShopServices.Interfaces;

namespace priceapp.proxy.Controllers;

public class CategoriesController
{
    private readonly ICategoriesService _categoriesService;
    private readonly IAtbService _atbService;

    public CategoriesController(ICategoriesService categoriesService, IAtbService atbService)
    {
        _categoriesService = categoriesService;
        _atbService = atbService;
    }

    public async Task<List<AtbCategoryModel>> GetAtbCategoriesAsync()
    {
        return await _categoriesService.GetAtbCategoriesAsync();
    }

    public async Task ActualizeCategoriesAsync(int shopId)
    {
        if (shopId == 3)
        {
            var categories = await _atbService.GetCategoriesAsync();
            await _categoriesService.InsertAsync(categories);
        }
    }
}