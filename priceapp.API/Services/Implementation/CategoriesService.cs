using AutoMapper;
using priceapp.API.Models;
using priceapp.API.Repositories.Interfaces;
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
}