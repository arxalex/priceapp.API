using AutoMapper;
using priceapp.API.Models;
using priceapp.API.Repositories.Interfaces;
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

        var rates = StringUtil.RateItemsByKeywords(search, items.Select(x => x.Label).ToList());

        return items.OrderBy(x => rates[x.Id]).ToList().GetRange(from, to - from);
    }

    public async Task<ItemModel> GetItemByIdAsync(int id)
    {
        return _mapper.Map<ItemModel>(await _itemsRepository.GetItemByIdAsync(id));
    }

    public async Task<List<ItemExtendedModel>> GetItemsExtendedByCategoryAsync(int categoryId, int from, int to)
    {
        var categories = await _categoriesService.GetChildCategoriesAsync(categoryId);
        return _mapper.Map<List<ItemExtendedModel>>(
            await _itemsRepository.GetItemExtendedByCategoriesAsync(categories.Select(x => x.Id).Prepend(categoryId),
                from, to));
    }

    public async Task<List<ItemExtendedModel>> GetItemsExtendedByCategoryAndLocationAsync(int categoryId,
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
        var rates = StringUtil.RateItemsByKeywords(search, items.Select(x => x.Label).ToList());

        return items.OrderBy(x => rates[x.Id]).ToList().GetRange(from, to - from);
    }

    public async Task<List<ItemExtendedModel>> SearchItemsExtendedByLocationAsync(string search, double xCord,
        double yCord, double radius,
        int from, int to)
    {
        var keywords = StringUtil.NameToKeywords(search);
        var filials = await _filialsService.GetFilialsByLocationAsync(xCord, yCord, radius);
        var items = _mapper.Map<List<ItemExtendedModel>>(
            await _itemsRepository.SearchItemsExtendedByLocationAsync(keywords, filials.Select(x => x.Id)));
        var rates = StringUtil.RateItemsByKeywords(search, items.Select(x => x.Label).ToList());

        return items.OrderBy(x => rates[x.Id]).ToList().GetRange(from, to - from);
    }

    public async Task<List<ItemModel>> SearchItemsByCategoryAsync(string search, int categoryId, int from, int to)
    {
        var keywords = StringUtil.NameToKeywords(search);
        var categories = await _categoriesService.GetChildCategoriesAsync(categoryId);
        var items = _mapper.Map<List<ItemModel>>(
            await _itemsRepository.GetItemsByKeywordsAndCategoryAsync(keywords,
                categories.Select(x => x.Id).Prepend(categoryId)));

        var rates = StringUtil.RateItemsByKeywords(search, items.Select(x => x.Label).ToList());
        var itemsOrdered = items.OrderBy(x => rates[x.Id]).ToList();

        return itemsOrdered.GetRange(from, to - from);
    }

    public async Task<List<ItemModel>> GetItemsByShopAndCategoryAsync(int shopId, int categoryId, int from, int to)
    {
        return shopId switch
        {
            1 => _mapper.Map<List<ItemModel>>(await _silpoService.GetItemsByCategoryAsync(categoryId, from, to)),
            2 => _mapper.Map<List<ItemModel>>(await _foraService.GetItemsByCategoryAsync(categoryId, from, to)),
            3 => _mapper.Map<List<ItemModel>>(await _atbService.GetItemsByCategoryAsync(categoryId, from, to)),
            _ => new List<ItemModel>()
        };
    }

    public async Task<ItemExtendedModel> GetItemExtendedByIdAsync(int id)
    {
        return _mapper.Map<ItemExtendedModel>(
            await _itemsRepository.GetItemExtendedAsync(id));
    }

    public async Task<ItemExtendedModel> GetItemExtendedByIdAndLocationAsync(int id, double xCord, double yCord,
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

            var rates = StringUtil.RateItemsByKeywords(search, items.Select(x => x.Label).ToList());

            itemsList.Add(items.OrderBy(x => rates[x.Id]).ToList().GetRange(from, to - from));
        }

        return itemsList;
    }
}