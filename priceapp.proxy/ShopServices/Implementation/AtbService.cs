using System.Net;
using System.Text.Json;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Connections;
using priceapp.proxy.Services.Interfaces;
using priceapp.proxy.Services.Models;
using priceapp.proxy.ShopServices.Interfaces;
using priceapp.proxy.ShopServices.Models;
using RestSharp;

namespace priceapp.proxy.ShopServices.Implementation;

public class AtbService : IAtbService
{
    private readonly RestClient _client;
    private readonly IFilialsService _filialsService;
    private readonly IItemsService _itemsService;
    private readonly ICategoriesService _categoriesService;

    private const int ShopId = 3;
    private DateTime _filialsLastUpdatedTime;
    private DateTime _itemsLastUpdatedTime;
    private DateTime _categoriesLastUpdatedTime;
    private List<AtbFilialModel> _filials;
    private List<AtbItemModel> _items;
    private List<AtbCategoryModel> _categories;

    private List<AtbFilialModel> Filials
    {
        get
        {
            if (DateTime.Now - _filialsLastUpdatedTime <= TimeSpan.FromMinutes(30)) return _filials;
            _filials = _filialsService.GetAtbFilialsAsync().Result;
            _filialsLastUpdatedTime = DateTime.Now;

            return _filials;
        }
        set
        {
            _filialsLastUpdatedTime = DateTime.Now;
            _filials = value;
        }
    }

    private List<AtbItemModel> Items
    {
        get
        {
            if (DateTime.Now - _itemsLastUpdatedTime <= TimeSpan.FromMinutes(30)) return _items;
            _items = _itemsService.GetAtbItemsAsync().Result;
            _itemsLastUpdatedTime = DateTime.Now;

            return _items;
        }
        set
        {
            _itemsLastUpdatedTime = DateTime.Now;
            _items = value;
        }
    }

    private List<AtbCategoryModel> Categories
    {
        get
        {
            if (DateTime.Now - _categoriesLastUpdatedTime <= TimeSpan.FromMinutes(30)) return _categories;
            _categories = _categoriesService.GetAtbCategoriesAsync().Result;
            _categoriesLastUpdatedTime = DateTime.Now;

            return _categories;
        }
        set
        {
            _categoriesLastUpdatedTime = DateTime.Now;
            _categories = value;
        }
    }

    public AtbService(IFilialsService filialsService, IItemsService itemsService, ICategoriesService categoriesService)
    {
        _filialsService = filialsService;
        _itemsService = itemsService;
        _categoriesService = categoriesService;
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://zakaz.atbmarket.com/")
        };

        _client = new RestClient(httpClient);
        _filialsLastUpdatedTime = DateTime.MinValue;
        _itemsLastUpdatedTime = DateTime.MinValue;
        _categoriesLastUpdatedTime = DateTime.MinValue;
        _filials = new List<AtbFilialModel>();
        _items = new List<AtbItemModel>();
        _categories = new List<AtbCategoryModel>();
    }

    public async Task<List<PriceModel>> GetPricesAsync(int categoryId, int filialId)
    {
        var filial = Filials.First(x => x.Id == filialId);
        var i = 0;
        var nextPage = true;
        var prices = new List<PriceModel>();
        var category = Categories.FirstOrDefault(x => x.Id == categoryId);

        const string xpathPriceQuery =
            "//div[@class='catalog-item__bottom']/div[contains(@class, 'catalog-item__product-price')]/data";
        const string xpathItemLinkQuery = "//div[@class='catalog-item__title']/a";
        const string xpathItem = "//article";

        if (category == null)
        {
            throw new Exception("No category");
        }

        while (nextPage)
        {
            var request =
                new RestRequest($"shop/catalog/wloadmore?cat={category.InternalId}&store={filial.InShopId}&page={i}");
            request.AddHeader("Cookie", $"store_id={filial.InShopId}");

            var response = await _client.ExecuteAsync(request);

            if (response.StatusCode != HttpStatusCode.OK || response.Content == null)
                throw new ConnectionAbortedException("Could not get data from Atb");

            var result = JsonSerializer.Deserialize<AtbItemsResponseModel>(response.Content);
            if (result == null) throw new ConnectionAbortedException("Could not parse data");

            var html = new HtmlDocument();
            if (result.markup == "")
            {
                break;
            }

            html.LoadHtml(result.markup);
            var htmlNodeRoot = html.DocumentNode;
            var arrayOfProducts = htmlNodeRoot!.SelectNodes(xpathItem);

            foreach (var product in arrayOfProducts!)
            {
                if (!int.TryParse(
                        product
                            .SelectSingleNode(xpathItemLinkQuery)
                            .GetAttributeValue("href", "///")
                            .Split('/')[3],
                        out var internalItemId))
                {
                    continue;
                }
                
                var item = Items.FirstOrDefault(x => x.InternalId == internalItemId);

                if (item == null)
                {
                    AtbItemModel newItem;
                    try
                    {
                        newItem = await GetItemAsync(internalItemId, categoryId, filial.InShopId);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    await _itemsService.InsertAsync(newItem);
                    Items = await _itemsService.GetAtbItemsAsync();
                    item = Items.First(x => x.InternalId == internalItemId);
                }

                prices.Add(new PriceModel
                {
                    ItemId = item.Id,
                    ShopId = ShopId,
                    Price = product.SelectSingleNode(xpathPriceQuery).GetAttributeValue("value", 0.0),
                    FilialId = filialId,
                    Id = -1,
                    Quantity = 1,
                    UpdateTime = DateTime.Now
                });
            }

            i++;
            nextPage = result.next_page;
        }

        return prices;
    }

    public async Task<List<AtbFilialModel>> GetFilialsAsync()
    {
        var filials = new List<AtbFilialModel>();
        var request = new RestRequest();
        var response = await _client.ExecuteAsync(request);

        if (response.StatusCode != HttpStatusCode.OK || string.IsNullOrEmpty(response.Content))
            throw new ConnectionAbortedException("Could not get data from Atb");

        var html = new HtmlDocument();
        html.LoadHtml(response.Content);

        const string xpathRegionQuery = "//*[@id='region']/option";
        const string xpathCitiesQuery = "//option";

        var htmlNodeRoot = html.DocumentNode;

        var regionNodes = htmlNodeRoot.SelectNodes(xpathRegionQuery);

        foreach (var regionNode in regionNodes)
        {
            var internalRegionId = regionNode.GetAttributeValue("value", 0);

            if (internalRegionId == 0)
            {
                continue;
            }

            var requestRegion = new RestRequest($"site/getregion", Method.Post);
            requestRegion.AddHeader("X-Requested-With", "XMLHttpRequest");
            requestRegion.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            requestRegion.AddParameter("id", $"{internalRegionId}");

            var responseRegion = await _client.ExecuteAsync(requestRegion);

            if (responseRegion.StatusCode != HttpStatusCode.OK || string.IsNullOrEmpty(responseRegion.Content))
                throw new ConnectionAbortedException("Could not get data from Atb");

            var htmlRegion = new HtmlDocument();
            htmlRegion.LoadHtml(responseRegion.Content);
            var htmlNodeRootRegion = html.DocumentNode;

            var cityNodes = htmlNodeRootRegion.SelectNodes(xpathCitiesQuery);
            foreach (var cityNode in cityNodes)
            {
                var internalCityId = cityNode.GetAttributeValue("value", 0);

                if (internalCityId == 0)
                {
                    continue;
                }

                var requestCity = new RestRequest($"site/getstore", Method.Post);
                requestCity.AddHeader("X-Requested-With", "XMLHttpRequest");
                requestCity.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                requestCity.AddParameter("store", $"{internalCityId}");

                var responseCity = await _client.ExecuteAsync(requestCity);

                if (responseCity.StatusCode == HttpStatusCode.Found && responseCity.Headers != null)
                {
                    var redirectHeader = responseCity
                        .Headers
                        .FirstOrDefault(x => x.Name == "X-Redirect");
                    if (redirectHeader == null || redirectHeader.Value == null || !int.TryParse(((string)redirectHeader.Value).Split("?id=")[1], out var internalFilialId))
                    {
                        continue;
                    }

                    if (Filials.Any(x => x.InShopId == internalFilialId)) continue;
                    var requestFilial = new RestRequest($"shop/catalog/wdelivery?store_id={internalFilialId}");
                    var responseFilial = await _client.ExecuteAsync(requestFilial);

                    if (responseFilial.StatusCode != HttpStatusCode.OK || string.IsNullOrEmpty(responseFilial.Content))
                        throw new ConnectionAbortedException("Could not get data from Atb");

                    var resultFilial = JsonSerializer.Deserialize<AtbFilialsResponseModel>(responseFilial.Content);
                    if (resultFilial == null) throw new ConnectionAbortedException("Could not parse data");
                    
                    var streetAndHouse = resultFilial.@out.address
                        .Split(' ', 2)[1]
                        .Split(", ", 2);

                    var filial = new AtbFilialModel()
                    {
                        Id = -1,
                        City = cityNode.InnerText,
                        House = streetAndHouse[1],
                        InShopId = internalFilialId,
                        Label = resultFilial.@out.address.Split(" ", 2)[0],
                        Region = regionNode.InnerText + " обл.",
                        Street = streetAndHouse[0],
                        XCord = null,
                        YCord = null
                    };

                    filials.Add(filial);
                }
                else
                {
                    if (responseCity.StatusCode != HttpStatusCode.OK || string.IsNullOrEmpty(responseCity.Content))
                        throw new ConnectionAbortedException("Could not get data from Atb");
                    var resultCity = JsonSerializer.Deserialize<AtbStoreResponseModel>(responseCity.Content);
                    if (resultCity == null) throw new ConnectionAbortedException("Could not parse data");

                    var htmlCity = new HtmlDocument();
                    if (resultCity.optselect == "")
                    {
                        break;
                    }

                    htmlCity.LoadHtml(resultCity.optselect);
                    var htmlCityNodeRoot = htmlCity.DocumentNode;

                    var filialNodes = htmlCityNodeRoot.SelectNodes(xpathCitiesQuery);
                    foreach (var filialNode in filialNodes)
                    {
                        var internalFilialId = filialNode.GetAttributeValue("value", 0);

                        if (internalFilialId == 0)
                        {
                            continue;
                        }

                        if (Filials.Any(x => x.InShopId == internalFilialId)) continue;
                        var requestFilial = new RestRequest($"shop/catalog/wdelivery?store_id={internalFilialId}");
                        var responseFilial = await _client.ExecuteAsync(requestFilial);

                        if (responseFilial.StatusCode != HttpStatusCode.OK ||
                            string.IsNullOrEmpty(responseFilial.Content))
                            throw new ConnectionAbortedException("Could not get data from Atb");

                        var resultFilial = JsonSerializer.Deserialize<AtbFilialsResponseModel>(responseFilial.Content);
                        if (resultFilial == null) throw new ConnectionAbortedException("Could not parse data");
                        
                        var streetAndHouse = filialNode.InnerText
                            .Split(' ', 2)[1]
                            .Split(", ", 2);

                        var filial = new AtbFilialModel()
                        {
                            Id = -1,
                            City = cityNode.InnerText,
                            House = streetAndHouse[1],
                            InShopId = internalFilialId,
                            Label = resultFilial.@out.address.Split(" ", 2)[0],
                            Region = regionNode.InnerText + " обл.",
                            Street = streetAndHouse[0],
                            XCord = double.Parse(resultCity.coordinates.First(x => x.id == internalFilialId).lng),
                            YCord = double.Parse(resultCity.coordinates.First(x => x.id == internalFilialId).lat)
                        };

                        filials.Add(filial);
                    }
                }
            }
        }

        return filials;
    }

    public async Task<List<AtbCategoryModel>> GetCategoriesAsync(bool insertParent = true)
    {
        var categories = new List<AtbCategoryModel>();
        var request = new RestRequest();
        var response = await _client.ExecuteAsync(request);

        if (response.StatusCode != HttpStatusCode.OK || string.IsNullOrEmpty(response.Content))
            throw new ConnectionAbortedException("Could not get data from Atb");

        var html = new HtmlDocument();
        html.LoadHtml(response.Content);

        const string xpathCategoryQuery = "//li[@class='category-menu__item']";
        const string xpathCategoryLinkQuery = "//a[@class='category-menu__link']";
        var htmlNodeRoot = html.DocumentNode;

        var categoryNodes = htmlNodeRoot.SelectNodes(xpathCategoryQuery);

        foreach (var categoryNode in categoryNodes)
        {
            var categoryLinkNode = categoryNode.SelectSingleNode(xpathCategoryLinkQuery);
            if (!int.TryParse(categoryLinkNode
                    .GetAttributeValue("href", "///")
                    .Split('/')[3], out var internalCategoryId) 
                || internalCategoryId == 388)
            {
                continue;
            }

            if (Categories.All(x => x.InternalId != internalCategoryId))
            {
                if (insertParent)
                {
                    await _categoriesService.InsertAsync(new AtbCategoryModel()
                    {
                        Id = -1,
                        InternalId = internalCategoryId,
                        Label = categoryLinkNode.InnerText,
                        Parent = null
                    });
                    Categories = await _categoriesService.GetAtbCategoriesAsync();
                }
                else
                {
                    categories.Add(new AtbCategoryModel()
                    {
                        Id = -1,
                        InternalId = internalCategoryId,
                        Label = categoryLinkNode.InnerText,
                        Parent = null
                    });
                }
            }

            const string xpathSubCategoryQuery = "//a[@class='submenu__link']";
            var subCategoriesLinkNodes = categoryNode.SelectNodes(xpathSubCategoryQuery);

            foreach (var subCategoryLinkNode in subCategoriesLinkNodes)
            {
                if (!int.TryParse(subCategoryLinkNode
                        .GetAttributeValue("href", "///")
                        .Split('/')[3], out var internalSubCategoryId) 
                    || internalSubCategoryId == 388)
                {
                    continue;
                }

                if (Categories.Any(x => x.InternalId == internalSubCategoryId)) continue;
                
                categories.Add(new AtbCategoryModel()
                {
                    Id = -1,
                    InternalId = internalSubCategoryId,
                    Label = subCategoryLinkNode.InnerText,
                    Parent = Categories.FirstOrDefault(x => x.InternalId == internalCategoryId)?.Id
                });
            }
        }

        return categories;
    }

    public async Task<List<AtbItemModel>> GetItemsAsync(int internalCategoryId, int internalFilialId = 1154)
    {
        var internalItemIds = await GetInternalItemIds(internalCategoryId, internalFilialId);
        var items = new List<AtbItemModel>();
        foreach (var internalItemId in internalItemIds)
        {
            if (Items.Any(x => x.InternalId == internalItemId)) continue;
            try
            {
                items.Add(await GetItemAsync(internalItemId, internalCategoryId, internalFilialId));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        return items;
    }

    private async Task<IEnumerable<int>> GetInternalItemIds(int internalCategoryId, int internalFilialId = 1154)
    {
        var internalItemIds = new List<int>();
        var i = 0;
        var nextPage = true;
        
        const string xpathItemLinkQuery = "//div[@class='catalog-item__title']/a";
        const string xpathItem = "//article";
        
        while (nextPage) {
            var request =
                new RestRequest($"shop/catalog/wloadmore?cat={internalCategoryId}&store={internalFilialId}&page={i}");
            request.AddHeader("Cookie", $"store_id={internalFilialId}");

            var response = await _client.ExecuteAsync(request);

            if (response.StatusCode != HttpStatusCode.OK || response.Content == null)
                throw new ConnectionAbortedException("Could not get data from Atb");

            var result = JsonSerializer.Deserialize<AtbItemsResponseModel>(response.Content);
            if (result == null) throw new ConnectionAbortedException("Could not parse data");

            var html = new HtmlDocument();
            if (result.markup == "")
            {
                break;
            }

            html.LoadHtml(result.markup);
            var htmlNodeRoot = html.DocumentNode;
            var arrayOfProducts = htmlNodeRoot!.SelectNodes(xpathItem);

            foreach (var product in arrayOfProducts!)
            {
                if (!int.TryParse(
                        product
                            .SelectSingleNode(xpathItemLinkQuery)
                            .GetAttributeValue("href", "///")
                            .Split('/')[3],
                        out var internalItemId))
                {
                    continue;
                }
                
                internalItemIds.Add(internalItemId);
            }

            i++;
            nextPage = result.next_page;
        }

        return internalItemIds;
    }

    private async Task<AtbItemModel> GetItemAsync(int internalItemId, int parentCategoryId, int internalFilialId = 1154)
    {
        var request = new RestRequest($"product/{internalFilialId}/{internalItemId}");
        request.AddHeader("Cookie", $"store_id={internalFilialId}");

        var response = await _client.ExecuteAsync(request);

        if (response.StatusCode != HttpStatusCode.OK || string.IsNullOrEmpty(response.Content))
            throw new ConnectionAbortedException("Could not get data from Atb");

        var html = new HtmlDocument();
        html.LoadHtml(response.Content);
        var htmlNodeRoot = html.DocumentNode;

        const string xpathNameQuery = "//h1[@class='page-title']";
        const string xpathImageQuery = "//div[@class='cardproduct-tabs__item']/picture/source";
        const string xpathCategoryQuery = "//li[@class='breadcrumbs__item']/a";
        const string xpathCharacteristicNameQuery = "//div[@class='product-characteristics__name']";
        const string xpathCharacteristicValueQuery = "//div[@class='product-characteristics__value']";
        const string xpathCharacteristicsQuery = "//div[@class='product-characteristics__item']";
        
        var categoryNode = htmlNodeRoot.SelectNodes(xpathCategoryQuery)[^1];

        if (!int.TryParse(categoryNode
                .GetAttributeValue("href", "///")
                .Split('/')[3], out var internalCategoryId))
        {
            throw new Exception("InternalId is not valid");
        }
        
        var category = Categories.FirstOrDefault(x => x.InternalId == internalCategoryId);
        if (category == null)
        {
            var newCategory = new AtbCategoryModel
            {
                Label = categoryNode.InnerText,
                Id = -1,
                InternalId = internalCategoryId,
                Parent = parentCategoryId
            };
            await _categoriesService.InsertAsync(newCategory);
            Categories = await _categoriesService.GetAtbCategoriesAsync();
            category = Categories.First(x => x.InternalId == internalCategoryId);
        }

        var characteristicNodes = htmlNodeRoot.SelectNodes(xpathCharacteristicsQuery);

        string? country = null;
        string? brand = null;

        foreach (var characteristic in characteristicNodes)
        {
            switch (characteristic.SelectSingleNode(xpathCharacteristicNameQuery).InnerText)
            {
                case "Країна":
                    country = characteristic.SelectSingleNode(xpathCharacteristicValueQuery).InnerText;
                    break;
                case "Торгова марка":
                    brand = characteristic.SelectSingleNode(xpathCharacteristicValueQuery).InnerText;
                    break;
            }
        }

        return new AtbItemModel
        {
            Id = -1,
            Brand = brand,
            Category = category.Id,
            Country = country,
            Image = htmlNodeRoot.SelectSingleNode(xpathImageQuery).GetAttributeValue("srcset", ""),
            InternalId = internalItemId,
            Label = htmlNodeRoot.SelectSingleNode(xpathNameQuery).InnerText
        };
    }
}