using AutoMapper;
using priceapp.API.Models;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Repositories.Models;
using priceapp.API.Services.Interfaces;
using priceapp.API.ShopServices.Interfaces;

namespace priceapp.API.Services.Implementation;

public class CategoryLinksService : ICategoryLinksService
{
    private readonly ISilpoService _silpoService;
    private readonly IForaService _foraService;
    private readonly IAtbService _atbService;
    private readonly IMapper _mapper;
    private readonly ICategoryLinksRepository _categoryLinksRepository;
    private readonly IShopsService _shopsService;

    public CategoryLinksService(ISilpoService silpoService, IForaService foraService, IAtbService atbService, IMapper mapper, ICategoryLinksRepository categoryLinksRepository, IShopsService shopsService)
    {
        _silpoService = silpoService;
        _foraService = foraService;
        _atbService = atbService;
        _mapper = mapper;
        _categoryLinksRepository = categoryLinksRepository;
        _shopsService = shopsService;
    }

    public async Task<List<CategoryLinkModel>> GetNewCategoryLinksAsync(int shopId)
    {
        var categories = shopId switch
        {
            1 => await _silpoService.GetCategoryLinksAsync(),
            2 => await _foraService.GetCategoryLinksAsync(),
            3 => await _atbService.GetCategoryLinksAsync(),
            _ => new List<CategoryLinkModel>()
        };

        return categories;
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

    public async Task ActualizeCategoryLinksAsync()
    {
        var shops = await _shopsService.GetShopsAsync();
        var links = new List<CategoryLinkModel>();
        foreach (var shop in shops)
        {
            links.AddRange(await GetNewCategoryLinksAsync(shop.Id));
        }

        await InsertOrUpdateCategoryLinksAsync(links);
    }

    public async Task InsertOrUpdateCategoryLinksAsync(List<CategoryLinkModel> links)
    {
        await _categoryLinksRepository.InsertOrUpdateCategoryLinksAsync(_mapper.Map<List<CategoryLinkRepositoryModel>>(links));
    }
}