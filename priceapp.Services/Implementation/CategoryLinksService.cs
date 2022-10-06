using AutoMapper;
using priceapp.Models;
using priceapp.Repositories.Interfaces;
using priceapp.Repositories.Models;
using priceapp.Services.Interfaces;

namespace priceapp.Services.Implementation;

public class CategoryLinksService : ICategoryLinksService
{
    private readonly IMapper _mapper;
    private readonly ICategoryLinksRepository _categoryLinksRepository;
    private readonly IShopsService _shopsService;

    public CategoryLinksService(IMapper mapper, ICategoryLinksRepository categoryLinksRepository, IShopsService shopsService)
    {
        _mapper = mapper;
        _categoryLinksRepository = categoryLinksRepository;
        _shopsService = shopsService;
    }
    public async Task<List<CategoryLinkModel>> GetCategoryLinksAsync(int shopId, int categoryId)
    {
        return _mapper.Map<List<CategoryLinkModel>>(
            await _categoryLinksRepository.GetCategoryLinksAsync(shopId, categoryId));
    }
    public async Task UpdateCategoryLinkAsync(CategoryLinkModel model)
    {
        await _categoryLinksRepository.UpdateCategoryLinkAsync(_mapper.Map<CategoryLinkRepositoryModel>(model));
    }
    public async Task InsertCategoryLinkAsync(CategoryLinkModel model)
    {
        await _categoryLinksRepository.InsertCategoryLinkAsync(_mapper.Map<CategoryLinkRepositoryModel>(model));
    }
    public async Task<CategoryLinkModel> GetCategoryLinkAsync(int shopId, int inShopId)
    {
        return _mapper.Map<CategoryLinkModel>(
            await _categoryLinksRepository.GetCategoryLinkAsync(shopId, inShopId));
    }
    public async Task<List<CategoryLinkModel>> GetCategoryLinksAsync(int shopId)
    {
        return _mapper.Map<List<CategoryLinkModel>>(await _categoryLinksRepository.GetCategoryLinksAsync(shopId));
    }

    public async Task InsertOrUpdateCategoryLinksAsync(List<CategoryLinkModel> links)
    {
        await _categoryLinksRepository.InsertOrUpdateCategoryLinksAsync(_mapper.Map<List<CategoryLinkRepositoryModel>>(links));
    }
}