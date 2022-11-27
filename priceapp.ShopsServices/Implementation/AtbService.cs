using AutoMapper;
using Microsoft.Extensions.Logging;
using priceapp.Models;
using priceapp.proxy.Controllers;
using priceapp.Repositories.Interfaces;
using priceapp.Services.Interfaces;
using priceapp.ShopsServices.Interfaces;

namespace priceapp.ShopsServices.Implementation;

public class AtbService : IAtbService
{
    private readonly IBrandsService _brandsService;
    private readonly ICountriesService _countriesService;
    private readonly IItemLinksService _itemLinksService;
    private readonly ILogger<AtbService> _logger;
    private readonly IFilialsRepository _filialsRepository;
    private readonly IMapper _mapper;
    private readonly ICategoryLinksRepository _categoryLinksRepository;
    private readonly ICategoriesService _categoriesService;
    private readonly PricesController _pricesController;
    private readonly ItemsController _itemsController;
    private readonly FilialsController _filialsController;
    private readonly CategoriesController _categoriesController;
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

    public AtbService(IItemLinksService itemLinksService, ICountriesService countriesService,
        IBrandsService brandsService, ILogger<AtbService> logger, IFilialsRepository filialsRepository, IMapper mapper,
        ICategoryLinksRepository categoryLinksRepository, ICategoriesService categoriesService,
        PricesController pricesController, ItemsController itemsController, FilialsController filialsController,
        CategoriesController categoriesController)
    {
        _itemLinksService = itemLinksService;
        _countriesService = countriesService;
        _brandsService = brandsService;
        _logger = logger;
        _filialsRepository = filialsRepository;
        _mapper = mapper;
        _categoryLinksRepository = categoryLinksRepository;
        _categoriesService = categoriesService;
        _pricesController = pricesController;
        _itemsController = itemsController;
        _filialsController = filialsController;
        _categoriesController = categoriesController;
        _itemLinksLastUpdatedTime = DateTime.MinValue;
        _itemLinks = new List<ItemLinkModel>();
    }

    public async Task<List<ItemShopModel>> GetItemsByCategoryAsync(int proxyCategoryId, int from, int to)
    {
        _logger.LogInformation("Start Atb GetItemsByCategoryAsync. proxyCategoryId: {ProxyCategoryId}", proxyCategoryId);
        var resultItems = await _itemsController.GetAtbItemsAsync(proxyCategoryId, from, to);

        var inTableItems = await _itemLinksService.GetItemLinksAsync(ShopId);
        ItemLinks = inTableItems;
        var notHandledResult = resultItems.Where(item => !inTableItems.Exists(x => x.InShopId == item.Id));

        var items = new List<ItemShopModel>();
        var categories = await _categoriesService.GetCategoriesAsync();
        var categoryLinks =
            _mapper.Map<List<CategoryLinkModel>>(await _categoryLinksRepository.GetCategoryLinksAsync(ShopId));
        var brands = await _brandsService.GetBrandsAsync();
        var countries = await _countriesService.GetCountriesAsync();

        foreach (var value in notHandledResult)
        {
            CategoryModel? categoryModel = null;
            CategoryLinkModel? categoryLinkModel = null;
            try
            {
                categoryLinkModel = categoryLinks.FirstOrDefault(x => x.CategoryShopId == value.Category);
                categoryModel = categories.FirstOrDefault(x => x.Id == categoryLinkModel?.CategoryId);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.Message);
            }

            var brandModel = value.Brand is { Length: > 0 }
                ? brands.FirstOrDefault(x => x.Label == value.Brand)
                : new BrandModel
                {
                    Id = 0,
                    Label = "Без ТМ",
                    Short = "Без ТМ"
                };
            var countryModel = value.Country is { Length: > 0 }
                ? countries.FirstOrDefault(x => x.Label == value.Country)
                : new CountryModel
                {
                    Id = 0,
                    Label = "Не вказано",
                    Short = "??"
                };

            var internalCategoryLabel = "";
            if (categoryLinkModel == null)
            {
                //TODO: add getting internalCategoryLabel
            }

            items.Add(new ItemShopModel
            {
                Item = new ItemModel
                {
                    Id = -1,
                    Label = value.Label,
                    Image = value.Image,
                    Category = categoryModel?.Id ?? 0,
                    Brand = brandModel?.Id ?? 0,
                    Additional = new
                    {
                        Country = countryModel?.Id ?? 0
                    }
                },
                InShopId = value.Id,
                Brand = value.Brand,
                Country = value.Country,
                Category = categoryLinkModel != null ? categoryLinkModel.ShopCategoryLabel : internalCategoryLabel,
                Url = "https://zakaz.atbmarket.com/product/1154/" + value.InternalId,
                ShopId = ShopId
            });
        }
        _logger.LogInformation("End Atb GetItemsByCategoryAsync. Total items {ItemsCount}", items.Count);

        return items;
    }

    public async Task<List<PriceModel>> GetPricesAsync(int categoryId, int proxyFilialId, int filialId)
    {
        _logger.LogInformation("Start Atb GetPricesAsync. categoryId: {CategoryId}, filialId: {FilialId}", categoryId, filialId);

        var proxyCategories =
            _mapper.Map<List<CategoryLinkModel>>(
                await _categoryLinksRepository.GetCategoryLinksAsync(ShopId, categoryId));
        var items = new List<proxy.Models.PriceModel>();
        foreach (var proxyCategory in proxyCategories)
        {
            items.AddRange(await _pricesController.GetPricesAsync(proxyCategory.Id, ShopId, proxyFilialId));
        }
        
        var prices = (from price in items
            join link in ItemLinks on price.ItemId equals link.InShopId
            select new PriceModel()
            {
                FilialId = filialId,
                Price = price.Price,
                Quantity = price.Quantity,
                PriceFactor = null,
                ShopId = ShopId,
                Id = -1,
                ItemId = link.ItemId
            }).ToList();
        
        _logger.LogInformation("End Atb GetPricesAsync. Total items {PricesCount}", prices.Count);

        return prices;
    }

    public async Task<List<FilialModel>> GetFilialsAsync()
    {
        _logger.LogInformation("Start Atb GetFilialsAsync");

        var inTableItems = _mapper.Map<List<FilialModel>>(await _filialsRepository.GetFilialsAsync(ShopId));
        var result = (await _filialsController.GetAtbFilialsAsync())
            .Where(filial => !inTableItems.Exists(x => x.InShopId == filial.Id) && filial.XCord != null &&
                             filial.YCord != null);
        
        var filials = result.Select(x => new FilialModel()
        {
            Id = -1,
            City = x.City,
            House = x.House,
            InShopId = x.Id,
            Label = x.Label,
            Region = x.Region,
            ShopId = ShopId,
            Street = x.Street,
            XCord = x.XCord ?? 0,
            YCord = x.YCord ?? 0
        }).ToList();
        
        _logger.LogInformation("End Atb GetFilialsAsync. Total items {FilialsCount}", filials.Count);

        return filials;
    }

    public async Task<List<CategoryLinkModel>> GetCategoryLinksAsync()
    {
        _logger.LogInformation("Start Atb GetCategoryLinksAsync");

        var inTableItems = await _categoryLinksRepository.GetCategoryLinksAsync(ShopId);
        
        var categoryLinks = (await _categoriesController.GetAtbCategoriesAsync())
            .Where(category => !inTableItems.Exists(x => x.categoryshopid == category.Id))
            .Select(x => new CategoryLinkModel()
            {
                Id = -1,
                CategoryId = 0,
                CategoryShopId = x.Id,
                ShopCategoryLabel = x.Label,
                ShopId = ShopId
            }).ToList();
        
        _logger.LogInformation("End Atb GetCategoryLinksAsync. Total items {CategoryLinksCount}", categoryLinks.Count);

        return categoryLinks;
    }
}