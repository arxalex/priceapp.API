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
    private readonly IItemLinksService _itemLinksService;
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;
    private readonly ILogger<AtbService> _logger;

    public AtbService(IItemLinksService itemLinksService, ICategoriesService categoriesService,
        ICountriesService countriesService, IBrandsService brandsService, ILogger<AtbService> logger)
    {
        _itemLinksService = itemLinksService;
        _categoriesService = categoriesService;
        _countriesService = countriesService;
        _brandsService = brandsService;
        _logger = logger;
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

        var inTableItems = await _itemLinksService.GetItemLinksAsync(3);
        var handledResult = resultItems.Where(item => !inTableItems.Exists(x => x.InShopId == item.id)).ToList();

        var items = new List<ItemShopModel>();
        var categories = await _categoriesService.GetCategoriesAsync();
        var categoryLinks = await _categoriesService.GetCategoryLinksAsync(1);
        var brands = await _brandsService.GetBrandsAsync();
        var countries = await _countriesService.GetCountriesAsync();
        
        foreach (var value in handledResult)
        {
            CategoryModel? categoryModel = null;
            CategoryLinkModel? categoryLinkModel = null;
            try
            {
                categoryLinkModel = categoryLinks.FirstOrDefault(x => x.CategoryShopId == value.category);
                categoryModel = categories.FirstOrDefault(x => x.Id == categoryLinkModel?.CategoryId);
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
            }
            var brandModel = value.brand != null && value.brand.Length > 0
                ? brands.FirstOrDefault(x => x.Label == value.brand)
                : new BrandModel
                {
                    Id = 0,
                    Label = "Без ТМ",
                    Short = "Без ТМ"
                };
            var countryModel = value.country != null && value.country.Length > 0
                ? countries.FirstOrDefault(x => x.Label == value.country)
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
                    Id = -1,
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
                Category = categoryLinkModel != null ? categoryLinkModel.ShopCategoryLabel : value.categorylabel,
                Url = "https://zakaz.atbmarket.com/product/1154/" + value.internalid,
                ShopId = 3
            });
        }

        return items;
    }
}