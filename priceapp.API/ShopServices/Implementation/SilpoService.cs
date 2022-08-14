using System.Globalization;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Connections;
using priceapp.API.Models;
using priceapp.API.Services.Interfaces;
using priceapp.API.ShopServices.Interfaces;
using priceapp.API.ShopServices.Models;
using priceapp.API.Utils;
using RestSharp;

namespace priceapp.API.ShopServices.Implementation;

public class SilpoService : ISilpoService
{
    private readonly IBrandsService _brandsService;
    private readonly ICategoriesService _categoriesService;
    private readonly RestClient _client;
    private readonly ICountriesService _countriesService;
    private readonly IItemLinksService _itemLinksService;
    private readonly ILogger<SilpoService> _logger;


    public SilpoService(IItemLinksService itemLinksService, IBrandsService brandsService,
        ICategoriesService categoriesService,
        ICountriesService countriesService, ILogger<SilpoService> logger)
    {
        _itemLinksService = itemLinksService;
        _brandsService = brandsService;
        _categoriesService = categoriesService;
        _countriesService = countriesService;
        _logger = logger;
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.catalog.ecom.silpo.ua/")
        };

        _client = new RestClient(httpClient);
    }

    public async Task<List<ItemShopModel>> GetItemsByCategoryAsync(int categoryId, int from, int to,
        int filialId = 2043)
    {
        var json = JsonSerializer.Serialize(new
        {
            data = new
            {
                from,
                to,
                categoryId,
                filialId
            },
            method = "GetSimpleCatalogItems"
        });

        var request = new RestRequest("api/2.0/exec/EcomCatalogGlobal", Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddBody(json, "application/json");

        var response = await _client.ExecuteAsync(request);

        if (response.StatusCode != HttpStatusCode.OK || response.Content == null)
            throw new ConnectionAbortedException("Could not get data from Silpo");

        var result = JsonSerializer.Deserialize<SilpoCatalogItems>(response.Content);
        if (result == null) throw new ConnectionAbortedException("Could not parse data");

        var inTableItems = await _itemLinksService.GetItemLinksAsync(1);
        var handledResult = result.items.Where(item => !inTableItems.Exists(x => x.InShopId == item.id)).ToList();

        var items = new List<ItemShopModel>();
        foreach (var value in handledResult)
        {
            var packageObject = value.parameters?.FirstOrDefault(x => x.key == "packageType");
            var packageLabel = packageObject != null ? packageObject.value : "";
            var package = 0;
            var (units, unitShort) = NumericHelper.ParseNumberString(value.unit);
            if (value.parameters?.FirstOrDefault(x => x.key == "isWeighted") != null) package = 1;

            var brandObject = value.parameters?.FirstOrDefault(x => x.key == "trademark");
            var brandLabel = "Без ТМ";
            if (brandObject != null) brandLabel = brandObject.value;

            var calorieObject = value.parameters?.FirstOrDefault(x => x.key == "calorie");
            double? calorie = null;
            if (calorieObject != null) calorie = double.Parse(calorieObject.value.Split('/', 2)[0].Replace(',', '.'), CultureInfo.InvariantCulture);

            var carbohydratesObject = value.parameters?.FirstOrDefault(x => x.key == "carbohydrates");
            double? carbohydrates = null;
            if (carbohydratesObject != null) carbohydrates = double.Parse(carbohydratesObject.value.Replace(',', '.'), CultureInfo.InvariantCulture);
            var fatsObject = value.parameters?.FirstOrDefault(x => x.key == "fats");
            double? fats = null;
            if (fatsObject != null) fats = double.Parse(fatsObject.value.Replace(',', '.'), CultureInfo.InvariantCulture);

            var proteinsObject = value.parameters?.FirstOrDefault(x => x.key == "proteins");
            double? proteins = null;
            if (proteinsObject != null) proteins = double.Parse(proteinsObject.value.Replace(',', '.'), CultureInfo.InvariantCulture);

            var alcoholObject = value.parameters?.FirstOrDefault(x => x.key == "alcoholContent");
            double? alcohol = null;
            if (alcoholObject != null) alcohol = double.Parse(alcoholObject.value.Replace(',', '.'), CultureInfo.InvariantCulture);

            var countryObject = value.parameters?.FirstOrDefault(x => x.key == "country");
            var country = "";
            if (countryObject != null) country = countryObject.value;

            switch (unitShort)
            {
                case "кг":
                    if (package != 1) package = 5;
                    break;
                case "г":
                    if (package != 1) package = 5;
                    units /= 1000;
                    break;
                case "шт/уп":
                    package = 5;
                    break;
                case "л":
                    package = 6;
                    break;
                case "мл":
                    package = 6;
                    units /= 1000;
                    break;
                case "шт":
                    package = 9;
                    break;
            }

            CategoryModel? categoryModel;
            try
            {
                categoryModel = await _categoriesService.GetCategoryAsync(1, value.categories[^1].id);
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
                categoryModel = null;
            }
            CategoryLinkModel? categoryLinkModel;
            try
            {
                categoryLinkModel = await _categoriesService.GetCategoryLinkAsync(1, value.categories[^1].id);
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
                categoryLinkModel = null;
            }

            var brandModel = brandLabel.Length > 0 ? await _brandsService.SearchBrandAsync(brandLabel) : null;
            var countryModel = country.Length > 0 ? await _countriesService.SearchCountryAsync(country) : null;

            items.Add(new ItemShopModel
            {
                Item = new ItemModel
                {
                    Label = value.name,
                    Image = value.mainImage,
                    Category = categoryModel?.Id ?? 0,
                    Brand = brandModel?.Id ?? 0,
                    Package = package,
                    Units = units ?? 0,
                    Calorie = calorie,
                    Carbohydrates = carbohydrates,
                    Fat = fats,
                    Proteins = proteins,
                    Additional = new
                    {
                        Alcohol = alcohol,
                        Country = countryModel?.Id ?? 0
                    }
                },
                InShopId = value.id,
                Brand = brandLabel,
                Package = packageLabel,
                Country = country,
                Category = categoryLinkModel != null ? categoryLinkModel.ShopCategoryLabel : value.categories[^1].name,
                Url = "https://shop.silpo.ua/product/" + value.slug
            });
        }

        return items;
    }
}