using AutoMapper;
using priceapp.API.Models;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Services.Interfaces;

namespace priceapp.API.Services.Implementation;

public class ShopsService : IShopsService
{
    private readonly IMapper _mapper;
    private readonly IShopsRepository _shopsRepository;

    public ShopsService(IMapper mapper, IShopsRepository shopsRepository)
    {
        _mapper = mapper;
        _shopsRepository = shopsRepository;
    }

    public async Task<List<ShopModel>> GetShopsAsync()
    {
        return _mapper.Map<List<ShopModel>>(await _shopsRepository.GetShopsAsync());
    }
}