using System.Data;
using Dapper;
using priceapp.API.Models;
using priceapp.API.Repositories;
using priceapp.API.Services.Interfaces;
using priceapp.API.ShopServices.Interfaces;
using priceapp.API.ShopServices.Models;
using priceapp.API.Utils;

namespace priceapp.API.ShopServices.Implementation;

public class AtbService : IAtbService
{
    private const string Table = "pa_items_atb";
    private const string TableCategories = "pa_categories_atb";
    private const string TableFilials = "pa_filials_atb";
    private const string TablePrices = "pa_prices";
    private readonly IBrandsService _brandsService;
    private readonly ICategoriesService _categoriesService;
    private readonly ICountriesService _countriesService;
    private readonly IItemLinksService _itemLinksService;
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;
    private readonly ILogger<AtbService> _logger;
    private const int ShopId = 3;
    
    private List<ItemLinkModel> _itemLinks;
    private DateTime _itemLinksLastUpdatedTime;

    private List<ItemLinkModel> ItemLinks
    {
        get
        {
            if (DateTime.Now - _itemLinksLastUpdatedTime <= TimeSpan.FromMinutes(30)) return _itemLinks;
            _itemLinks = _itemLinksService.GetItemLinksAsync(ShopId).Result;
            _itemLinksLastUpdatedTime = DateTime.Now;

            return _itemLinks;
        }
        set
        {
            _itemLinksLastUpdatedTime = DateTime.Now;
            _itemLinks = value;
        }
    }

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
        _itemLinksLastUpdatedTime = DateTime.MinValue;
        _itemLinks = new List<ItemLinkModel>();
    }

    public async Task<List<ItemShopModel>> GetItemsByCategoryAsync(int proxyCategoryId, int from, int to)
    {
        var resultItems = await GetItems(proxyCategoryId, from, to);

        var inTableItems = await _itemLinksService.GetItemLinksAsync(ShopId);
        ItemLinks = inTableItems;
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
                ShopId = ShopId
            });
        }

        return items;
    }

    public async Task<List<PriceModel>> GetPrices(int categoryId, int filialId, int from = 0, int to = 10000)
    {
        var proxyCategories = await _categoriesService.GetCategoryLinksAsync(ShopId, categoryId);
        var items = new List<AtbItemModel>();
        foreach (var proxyCategory in proxyCategories)
        {
            items.AddRange(await GetItems(proxyCategory.Id, from, to));
        }

        return (from item in items
            join link in ItemLinks on item.id equals link.InShopId
            select new PriceModel()
            {
                FilialId = filialId,
                Price = item.price,
                Quantity = item.quantity,
                PriceFactor = null,
                ShopId = ShopId,
                Id = -1,
                ItemId = link.ItemId
            }).ToList();
    }

    private async Task<List<AtbCategoryModel>> GetChildCategories(int proxyCategoryId)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();

        const string query = $"select * from {TableCategories} where ";
        var resultByLevel = new List<List<AtbCategoryModel>>();
        var i = 1;
        resultByLevel.Add(new List<AtbCategoryModel>
        {
            new() { id = proxyCategoryId }
        });

        while (resultByLevel[i - 1].Count > 0)
        {
            var queryResult = query + DatabaseUtil.GetInQuery(resultByLevel[i - 1].Select(x => x.id), "`parent`");
            resultByLevel.Add((await connection.QueryAsync<AtbCategoryModel>(queryResult)).ToList());
            i++;
        }

        var result = new List<AtbCategoryModel>();

        for (var j = 1; j < resultByLevel.Count; j++) result.AddRange(resultByLevel[j]);

        return result;
    }

    private async Task<List<AtbItemModel>> GetItems(int proxyCategoryId, int from, int to)
    {
        var categoryIds = await GetChildCategories(proxyCategoryId);
        using var connection = _mySqlDbConnectionFactory.Connect();
        var whereQueryCategories = DatabaseUtil.GetInQuery(categoryIds.Select(x => x.id).Prepend(proxyCategoryId), "t.category");
        
        var query = @$"select t.id, t.internalid, t.label, t.image, t.category, t.brand, t.country, tc.label as categorylabel, 0 as price, 0 as quantity
                                from {Table} t 
                                left join {TableCategories} tc on t.category = tc.id
                                where {whereQueryCategories}
                                order by t.id
                                limit @limit 
                                offset @offset";
        
        var parameters = new DynamicParameters();
        parameters.Add("@category", proxyCategoryId, DbType.Int32);
        parameters.Add("@limit", to - from, DbType.Int32);
        parameters.Add("@offset", from, DbType.Int32);
        var resultItems = (await connection.QueryAsync<AtbItemModel>(query, parameters)).ToList();

        return resultItems;
    }
    
    private async Task<List<AtbItemModel>> GetItems(int proxyCategoryId, int proxyFilialId, int from, int to)
    {
        var categoryIds = await GetChildCategories(proxyCategoryId);
        using var connection = _mySqlDbConnectionFactory.Connect();
        var whereQueryCategories = DatabaseUtil.GetInQuery(categoryIds.Select(x => x.id).Prepend(proxyCategoryId), "t.category");
        
        var query = @$"select t.id, t.internalid, t.label, t.image, t.category, t.brand, t.country, tc.label as categorylabel, tp.price, tp.quantity
                                from {Table} t 
                                left join {TableCategories} tc on t.category = tc.id
                                left join {TablePrices} tp on tp.itemid = t.id
                                where tp.shopid = {ShopId}
                                and tp.filialid = @filialId
                                and {whereQueryCategories}
                                order by t.id
                                limit @limit 
                                offset @offset";
        
        var parameters = new DynamicParameters();
        parameters.Add("@filialId", proxyFilialId, DbType.Int32);
        parameters.Add("@category", proxyCategoryId, DbType.Int32);
        parameters.Add("@limit", to - from, DbType.Int32);
        parameters.Add("@offset", from, DbType.Int32);
        var resultItems = (await connection.QueryAsync<AtbItemModel>(query, parameters)).ToList();

        return resultItems;
    }
}