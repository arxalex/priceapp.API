using AutoMapper;
using priceapp.Models;
using priceapp.Repositories.Interfaces;
using priceapp.Repositories.Models;
using priceapp.Services.Interfaces;
using priceapp.Utils;

namespace priceapp.Services.Implementation;

public class ItemsService : IItemsService
{
    private readonly ICategoriesService _categoriesService;
    private readonly IFilialsService _filialsService;
    private readonly IItemsRepository _itemsRepository;
    private readonly IMapper _mapper;

    public ItemsService(IItemsRepository itemsRepository, IMapper mapper, ICategoriesService categoriesService,
        IFilialsService filialsService)
    {
        _itemsRepository = itemsRepository;
        _mapper = mapper;
        _categoriesService = categoriesService;
        _filialsService = filialsService;
    }

    public async Task<List<ItemModel>> SearchItemsAsync(string search, int from, int to)
    {
        var keywords = StringUtil.NameToKeywords(search);
        var items = _mapper.Map<List<ItemModel>>(await _itemsRepository.GetItemsAsync(keywords));

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
        return _mapper.Map<ItemModel>(await _itemsRepository.GetItemAsync(id));
    }

    public async Task<List<ItemExtendedModel>> GetItemsExtendedAsync(int categoryId, int from, int to)
    {
        var categories = await _categoriesService.GetChildCategoriesAsync(categoryId);
        return _mapper.Map<List<ItemExtendedModel>>(
            await _itemsRepository.GetItemExtendedAsync(categories.Select(x => x.Id).Prepend(categoryId),
                from, to));
    }

    public async Task<List<ItemExtendedModel>> GetItemsExtendedAsync(int categoryId,
        double xCord,
        double yCord, double radius, int from, int to)
    {
        var categories = await _categoriesService.GetChildCategoriesAsync(categoryId);
        var filials = await _filialsService.GetFilialsAsync(xCord,
            yCord, radius);
        return _mapper.Map<List<ItemExtendedModel>>(
            await _itemsRepository.GetItemsExtendedAsync(
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
        var filials = await _filialsService.GetFilialsAsync(xCord, yCord, radius);
        var items = _mapper.Map<List<ItemExtendedModel>>(
            await _itemsRepository.SearchItemsExtendedAsync(keywords, filials.Select(x => x.Id)));
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
            await _itemsRepository.GetItemsAsync(keywords,
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

    public async Task<ItemExtendedModel> GetItemExtendedAsync(int id)
    {
        return _mapper.Map<ItemExtendedModel>(
            await _itemsRepository.GetItemExtendedAsync(id));
    }

    public async Task<ItemExtendedModel> GetItemExtendedAsync(int id, double xCord, double yCord,
        double radius)
    {
        var filials = await _filialsService.GetFilialsAsync(xCord, yCord, radius);
        return _mapper.Map<ItemExtendedModel>(
            await _itemsRepository.GetItemExtendedAsync(id, filials.Select(x => x.Id)));
    }

    public async Task<List<List<ItemModel>>> SearchMultipleItemsAsync(List<string> searchList, int from, int to)
    {
        var itemsList = new List<List<ItemModel>>();
        foreach (var search in searchList)
        {
            var keywords = StringUtil.NameToKeywords(search);
            var items = _mapper.Map<List<ItemModel>>(await _itemsRepository.GetItemsAsync(keywords));

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

    public async Task<ItemModel> GetLastInsertedItemAsync()
    {
        return _mapper.Map<ItemModel>(await _itemsRepository.GetLastInsertedItemAsync());
    }
}