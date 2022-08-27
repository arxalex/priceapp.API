using AutoMapper;
using priceapp.API.Models;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Repositories.Models;
using priceapp.API.Services.Interfaces;

namespace priceapp.API.Services.Implementation;

public class ItemLinksService : IItemLinksService
{
    private readonly IItemsRepository _itemsRepository;
    private readonly ICategoriesService _categoriesService;
    private readonly IMapper _mapper;

    public ItemLinksService(IMapper mapper, IItemsRepository itemsRepository, ICategoriesService categoriesService)
    {
        _mapper = mapper;
        _itemsRepository = itemsRepository;
        _categoriesService = categoriesService;
    }

    public async Task<List<ItemLinkModel>> GetItemLinksAsync(int shopId)
    {
        return _mapper.Map<List<ItemLinkModel>>(await _itemsRepository.GetItemLinksAsync(shopId));
    }

    public async Task InsertItemLinkAsync(ItemLinkModel model)
    {
        await _itemsRepository.InsertItemLinkAsync(_mapper.Map<ItemLinkRepositoryModel>(model));
    }

    public async Task<List<ItemLinkModel>> GetItemLinksAsync(int shopId, int categoryId)
    {
        var categories = await _categoriesService.GetChildCategoriesAsync(categoryId);
        return _mapper.Map<List<ItemLinkModel>>(
            await _itemsRepository.GetItemLinksAsync(shopId, categories.Select(x => x.Id).Prepend(categoryId)));
    }
}