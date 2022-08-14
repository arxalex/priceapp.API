using AutoMapper;
using priceapp.API.Models;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Services.Interfaces;

namespace priceapp.API.Services.Implementation;

public class ItemLinksService : IItemLinksService
{
    private readonly IItemsRepository _itemsRepository;
    private readonly IMapper _mapper;

    public ItemLinksService(IMapper mapper, IItemsRepository itemsRepository)
    {
        _mapper = mapper;
        _itemsRepository = itemsRepository;
    }

    public async Task<List<ItemLinkModel>> GetItemLinksAsync(int shopId)
    {
        return _mapper.Map<List<ItemLinkModel>>(await _itemsRepository.GetItemLinksByShopAsync(shopId));
    }
}