using AutoMapper;
using priceapp.proxy.Models;
using priceapp.proxy.Repositories.Interfaces;
using priceapp.proxy.Repositories.Models;
using priceapp.proxy.Services.Interfaces;

namespace priceapp.proxy.Services.Implementation;

public class ItemsService : IItemsService
{
    private readonly IMapper _mapper;
    private readonly IItemsRepository _itemsRepository;
    private readonly ICategoriesService _categoriesService;

    public ItemsService(IMapper mapper, IItemsRepository itemsRepository, ICategoriesService categoriesService)
    {
        _mapper = mapper;
        _itemsRepository = itemsRepository;
        _categoriesService = categoriesService;
    }

    public async Task<List<AtbItemModel>> GetAtbItemsAsync()
    {
        return _mapper.Map<List<AtbItemModel>>(await _itemsRepository.GetAtbItemsAsync());

    }

    public async Task InsertAsync(AtbItemModel model)
    {
        await _itemsRepository.InsertAsync(_mapper.Map<AtbItemRepositoryModel>(model));
    }

    public async Task<List<AtbItemModel>> GetAtbItemsAsync(int categoryId, int from, int to)
    {
        var categoryIds = await _categoriesService.GetAtbChildCategoriesAsync(categoryId);
        return _mapper.Map<List<AtbItemModel>>(await _itemsRepository.GetAtbItemsAsync(categoryIds.Select(x => x.Id), from, to));
    }

    public async Task InsertAsync(List<AtbItemModel> models)
    {
        await _itemsRepository.InsertOrUpdateAsync(_mapper.Map<List<AtbItemRepositoryModel>>(models));
    }
}