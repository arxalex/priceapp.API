using AutoMapper;
using priceapp.proxy.Models;
using priceapp.proxy.Repositories.Interfaces;
using priceapp.proxy.Repositories.Models;
using priceapp.proxy.Services.Interfaces;

namespace priceapp.proxy.Services.Implementation;

public class PricesService : IPricesService
{
    private readonly IPricesRepository _pricesRepository;
    private readonly ICategoriesService _categoriesService;
    private readonly IMapper _mapper;

    public PricesService(IPricesRepository pricesRepository, IMapper mapper, ICategoriesService categoriesService)
    {
        _pricesRepository = pricesRepository;
        _mapper = mapper;
        _categoriesService = categoriesService;
    }

    public async Task<List<PriceModel>> GetPricesAsync(int categoryId, int shopId, int filialId)
    {
        var categoryIds = await _categoriesService.GetAtbChildCategoriesAsync(categoryId);

        return _mapper.Map<List<PriceModel>>(
            await _pricesRepository.GetPrices(categoryIds.Select(x => x.Id), shopId, filialId));
    }

    public async Task InsertAsync(List<PriceModel> models)
    {
        if (models.Count < 1) return;
        await _pricesRepository.InsertOrUpdateAsync(_mapper.Map<List<PriceRepositoryModel>>(models));
    }
}