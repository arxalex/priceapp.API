using AutoMapper;
using priceapp.API.Models;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Repositories.Models;
using priceapp.API.Services.Interfaces;

namespace priceapp.API.Services.Implementation;

public class CategoriesService : ICategoriesService
{
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IMapper _mapper;

    public CategoriesService(IMapper mapper, ICategoriesRepository categoriesRepository)
    {
        _mapper = mapper;
        _categoriesRepository = categoriesRepository;
    }

    public async Task<List<CategoryModel>> GetChildCategoriesAsync(int baseCategoryId)
    {
        return _mapper.Map<List<CategoryModel>>(await _categoriesRepository.GetChildCategoriesAsync(baseCategoryId));
    }

    public async Task<List<CategoryModel>> GetCategoriesAsync()
    {
        return _mapper.Map<List<CategoryModel>>(await _categoriesRepository.GetCategoriesAsync());
    }

    public async Task<CategoryModel> GetCategoryAsync(int shopId, int inShopId)
    {
        return _mapper.Map<CategoryModel>(
            await _categoriesRepository.GetCategoryAsync(shopId, inShopId));
    }

    public async Task InsertCategoryAsync(CategoryModel model)
    {
        await _categoriesRepository.InsertCategoryAsync(_mapper.Map<CategoryRepositoryModel>(model));
    }

    public async Task UpdateCategoryAsync(CategoryModel model)
    {
        await _categoriesRepository.UpdateCategoryAsync(_mapper.Map<CategoryRepositoryModel>(model));
    }

    public async Task<List<CategoryModel>> GetBaseCategoriesAsync()
    {
        return _mapper.Map<List<CategoryModel>>(await _categoriesRepository.GetBaseCategoriesAsync());
    }
}