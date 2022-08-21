using AutoMapper;
using priceapp.API.Models;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Repositories.Models;
using priceapp.API.Services.Interfaces;
using priceapp.API.ShopServices.Interfaces;
using priceapp.API.Utils;

namespace priceapp.API.Services.Implementation;

public class ItemsService : IItemsService
{
    private readonly IAtbService _atbService;
    private readonly ICategoriesService _categoriesService;
    private readonly IFilialsService _filialsService;
    private readonly IForaService _foraService;
    private readonly IItemsRepository _itemsRepository;
    private readonly IMapper _mapper;
    private readonly ISilpoService _silpoService;

    public ItemsService(IItemsRepository itemsRepository, IMapper mapper, ISilpoService silpoService,
        IForaService foraService, IAtbService atbService, ICategoriesService categoriesService,
        IFilialsService filialsService)
    {
        _itemsRepository = itemsRepository;
        _mapper = mapper;
        _silpoService = silpoService;
        _foraService = foraService;
        _atbService = atbService;
        _categoriesService = categoriesService;
        _filialsService = filialsService;
    }

    public async Task<List<ItemModel>> SearchItemsAsync(string search, int from, int to)
    {
        var keywords = StringUtil.NameToKeywords(search);
        var items = _mapper.Map<List<ItemModel>>(await _itemsRepository.GetItemsByKeywordsAsync(keywords));

        var rates = StringUtil.RateItemsByKeywords(search, items.Select(x => (x.Id, x.Label)).ToList());

        if (items.Count <= from)
        {
            return new List<ItemModel>();
        }

        var count = items.Count > to ? to - from : items.Count - from;

        return items.OrderByDescending(x => rates[x.Id]).ToList().GetRange(from, count);
    }

    public async Task<ItemModel> GetItemAsync(int id)
    {
        return _mapper.Map<ItemModel>(await _itemsRepository.GetItemByIdAsync(id));
    }

    public async Task<List<ItemExtendedModel>> GetItemsExtendedAsync(int categoryId, int from, int to)
    {
        var categories = await _categoriesService.GetChildCategoriesAsync(categoryId);
        return _mapper.Map<List<ItemExtendedModel>>(
            await _itemsRepository.GetItemExtendedByCategoriesAsync(categories.Select(x => x.Id).Prepend(categoryId),
                from, to));
    }

    public async Task<List<ItemExtendedModel>> GetItemsExtendedAsync(int categoryId,
        double xCord,
        double yCord, double radius, int from, int to)
    {
        var categories = await _categoriesService.GetChildCategoriesAsync(categoryId);
        var filials = await _filialsService.GetFilialsByLocationAsync(xCord,
            yCord, radius);
        return _mapper.Map<List<ItemExtendedModel>>(
            await _itemsRepository.GetItemsExtendedByCategoriesAndFilialsAsync(
                categories.Select(x => x.Id).Prepend(categoryId), filials.Select(x => x.Id), from, to));
    }

    public async Task<List<ItemExtendedModel>> SearchItemsExtendedAsync(string search, int from, int to)
    {
        var keywords = StringUtil.NameToKeywords(search);
        var items = _mapper.Map<List<ItemExtendedModel>>(
            await _itemsRepository.SearchItemsExtendedAsync(keywords));
        var rates = StringUtil.RateItemsByKeywords(search, items.Select(x => (x.Id, x.Label)).ToList());

        if (items.Count <= from)
        {
            return new List<ItemExtendedModel>();
        }

        var count = items.Count > to ? to - from : items.Count - from;

        return items.OrderByDescending(x => rates[x.Id]).ToList().GetRange(from, count);
    }

    public async Task<List<ItemExtendedModel>> SearchItemsExtendedAsync(string search, double xCord,
        double yCord, double radius,
        int from, int to)
    {
        var keywords = StringUtil.NameToKeywords(search);
        var filials = await _filialsService.GetFilialsByLocationAsync(xCord, yCord, radius);
        var items = _mapper.Map<List<ItemExtendedModel>>(
            await _itemsRepository.SearchItemsExtendedByLocationAsync(keywords, filials.Select(x => x.Id)));
        var rates = StringUtil.RateItemsByKeywords(search, items.Select(x => (x.Id, x.Label)).ToList());

        if (items.Count <= from)
        {
            return new List<ItemExtendedModel>();
        }

        var count = items.Count > to ? to - from : items.Count - from;

        return items.OrderByDescending(x => rates[x.Id]).ToList().GetRange(from, count);
    }

    public async Task<List<ItemModel>> SearchItemsAsync(string search, int categoryId, int from, int to)
    {
        var keywords = StringUtil.NameToKeywords(search);
        var categories = await _categoriesService.GetChildCategoriesAsync(categoryId);
        var items = _mapper.Map<List<ItemModel>>(
            await _itemsRepository.GetItemsByKeywordsAndCategoryAsync(keywords,
                categories.Select(x => x.Id).Prepend(categoryId)));

        var rates = StringUtil.RateItemsByKeywords(search, items.Select(x => (x.Id, x.Label)).ToList());
        var itemsOrdered = items.OrderByDescending(x => rates[x.Id]).ToList();

        if (items.Count <= from)
        {
            return new List<ItemModel>();
        }

        var count = items.Count > to ? to - from : items.Count - from;

        return itemsOrdered.GetRange(from, count);
    }

    public async Task<List<ItemShopModel>> GetShopItemsAsync(int shopId, int categoryId, int from, int to)
    {
        return shopId switch
        {
            1 => _mapper.Map<List<ItemShopModel>>(await _silpoService.GetItemsByCategoryAsync(categoryId, from, to)),
            2 => _mapper.Map<List<ItemShopModel>>(await _foraService.GetItemsByCategoryAsync(categoryId, from, to)),
            3 => _mapper.Map<List<ItemShopModel>>(await _atbService.GetItemsByCategoryAsync(categoryId, from, to)),
            _ => new List<ItemShopModel>()
        };
    }

    public async Task<ItemExtendedModel> GetItemExtendedAsync(int id)
    {
        return _mapper.Map<ItemExtendedModel>(
            await _itemsRepository.GetItemExtendedAsync(id));
    }

    public async Task<ItemExtendedModel> GetItemExtendedAsync(int id, double xCord, double yCord,
        double radius)
    {
        var filials = await _filialsService.GetFilialsByLocationAsync(xCord, yCord, radius);
        return _mapper.Map<ItemExtendedModel>(
            await _itemsRepository.GetItemExtendedByLocationAsync(id, filials.Select(x => x.Id)));
    }

    public async Task<List<List<ItemModel>>> SearchMultipleItemsAsync(List<string> searchList, int from, int to)
    {
        var itemsList = new List<List<ItemModel>>();
        foreach (var search in searchList)
        {
            var keywords = StringUtil.NameToKeywords(search);
            var items = _mapper.Map<List<ItemModel>>(await _itemsRepository.GetItemsByKeywordsAsync(keywords));

            var rates = StringUtil.RateItemsByKeywords(search, items.Select(x => (x.Id, x.Label)).ToList());

            if (items.Count <= from)
            {
                itemsList.Add(new List<ItemModel>());
                continue;
            }

            var count = items.Count > to ? to - from : items.Count - from;
            itemsList.Add(items.OrderByDescending(x => rates[x.Id]).ToList().GetRange(from, count));
        }

        return itemsList;
    }

    public async Task InsertItemAsync(ItemModel model)
    {
        await _itemsRepository.InsertItemAsync(_mapper.Map<ItemRepositoryModel>(model));
    }

    public async Task UpdateItemAsync(ItemModel model)
    {
        await _itemsRepository.UpdateItemAsync(_mapper.Map<ItemRepositoryModel>(model));
    }

    public async Task InsertItemLinkAsync(ItemLinkModel model)
    {
        await _itemsRepository.InsertItemLinkAsync(_mapper.Map<ItemLinkRepositoryModel>(model));
    }

    public async Task<ItemModel> GetLastInsertedItemAsync()
    {
        return _mapper.Map<ItemModel>(await _itemsRepository.GetLastInsertedItemAsync());
    }
}