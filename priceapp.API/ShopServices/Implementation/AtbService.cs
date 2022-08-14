using System.Data;
using Dapper;
using priceapp.API.Models;
using priceapp.API.Repositories;
using priceapp.API.Services.Interfaces;
using priceapp.API.ShopServices.Interfaces;
using priceapp.API.ShopServices.Models;

namespace priceapp.API.ShopServices.Implementation;

public class AtbService : IAtbService
{
    private const string Table = "pa_items_atb";
    private const string TableCategories = "pa_categories_atb";
    private readonly IBrandsService _brandsService;
    private readonly ICategoriesService _categoriesService;
    private readonly ICountriesService _countriesService;
    private readonly IItemsService _itemsService;
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;

    public AtbService(IItemsService itemsService, ICategoriesService categoriesService,
        ICountriesService countriesService, IBrandsService brandsService)
    {
        _itemsService = itemsService;
        _categoriesService = categoriesService;
        _countriesService = countriesService;
        _brandsService = brandsService;
        _mySqlDbConnectionFactory = new MySQLDbConnectionFactory(
            "server=priceapp.crjdcmsi5oyh.eu-central-1.rds.amazonaws.com;user=priceapp_admin;password=h9Fht9EiuE46AD7;database=arxalexc_priceapp_proxy");
    }

    public async Task<List<ItemShopModel>> GetItemsByCategoryAsync(int categoryId, int from, int to)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query =
            @$"select t.id, t.internalid, t.label, t.image, t.category, t.brand, t.country, tc.label as categorylabel
                                from {Table} t 
                                left join {TableCategories} tc on t.category = tc.id
                                where t.category = @category 
                                order by t.id
                                limit @limit 
                                offset @offset";
        var parameters = new DynamicParameters();
        parameters.Add("@category", categoryId, DbType.Int32);
        parameters.Add("@limit", to - from, DbType.Int32);
        parameters.Add("@offset", from, DbType.Int32);
        var resultItems = (await connection.QueryAsync<AtbItemModel>(query, parameters)).ToList();

        var inTableItems = await _itemsService.GetItemLinksAsync(1);
        var handledResult = resultItems.Where(item => !inTableItems.Exists(x => x.InShopId == item.id)).ToList();

        var items = new List<ItemShopModel>();
        foreach (var value in handledResult)
        {
            var categoryModel = await _categoriesService.GetCategoryByShopAndInShopIdAsync(1, value.category);
            var brandModel = value.brand != null
                ? await _brandsService.SearchBrandAsync(value.brand)
                : new BrandModel
                {
                    Id = 0,
                    Label = "Без ТМ",
                    Short = "Без ТМ"
                };
            var countryModel = value.country != null
                ? await _countriesService.SearchCountryAsync(value.country)
                : new CountryModel
                {
                    Id = 0,
                    Label = "Не вказано",
                    Short = "??"
                };

            items.Add(new ItemShopModel
            {
                Item = new ItemModel
                {
                    Label = value.label,
                    Image = value.image,
                    Category = categoryModel?.Id ?? 0,
                    Brand = brandModel?.Id ?? 0,
                    Additional = new
                    {
                        Country = countryModel?.Id ?? 0
                    }
                },
                InShopId = value.id,
                Brand = value.brand,
                Country = value.country,
                Category = value.categorylabel,
                Url = "https://zakaz.atbmarket.com/product/1154/" + value.internalid
            });
        }

        return items;
    }
}