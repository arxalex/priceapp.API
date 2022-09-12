using AutoMapper;
using priceapp.proxy.Repositories.Interfaces;
using priceapp.proxy.Repositories.Models;
using priceapp.proxy.Services.Interfaces;
using priceapp.proxy.Services.Models;

namespace priceapp.proxy.Services.Implementation;

public class CategoriesService : ICategoriesService
{
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IMapper _mapper;

    public CategoriesService(ICategoriesRepository categoriesRepository, IMapper mapper)
    {
        _categoriesRepository = categoriesRepository;
        _mapper = mapper;
    }

    public async Task<List<AtbCategoryModel>> GetAtbCategoriesAsync()
    {
        return _mapper.Map<List<AtbCategoryModel>>(await _categoriesRepository.GetAtbCategoriesAsync());
    }

    public async Task InsertAsync(AtbCategoryModel model)
    {
        await _categoriesRepository.InsertAsync(_mapper.Map<AtbCategoryRepositoryModel>(model));
    }

    public async Task<List<AtbCategoryModel>> GetAtbChildCategoriesAsync(int categoryId)
    {
        return _mapper.Map<List<AtbCategoryModel>>(await _categoriesRepository.GetAtbChildCategoriesAsync(categoryId));
    }

    public async Task InsertAsync(List<AtbCategoryModel> models)
    {
        await _categoriesRepository.InsertOrUpdateAsync(_mapper.Map<List<AtbCategoryRepositoryModel>>(models));
    }
}