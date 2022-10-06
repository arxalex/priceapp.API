using AutoMapper;
using priceapp.Models;
using priceapp.Repositories.Interfaces;
using priceapp.Services.Interfaces;

namespace priceapp.Services.Implementation;

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