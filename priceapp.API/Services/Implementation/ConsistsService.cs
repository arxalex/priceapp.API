using AutoMapper;
using priceapp.API.Models;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Services.Interfaces;

namespace priceapp.API.Services.Implementation;

public class ConsistsService : IConsistsService
{
    private readonly IConsistsRepository _consistsRepository;
    private readonly IMapper _mapper;

    public ConsistsService(IMapper mapper, IConsistsRepository consistsRepository)
    {
        _mapper = mapper;
        _consistsRepository = consistsRepository;
    }

    public async Task<List<ConsistModel>> GetConsistsAsync()
    {
        return _mapper.Map<List<ConsistModel>>(await _consistsRepository.GetConsistsAsync());
    }
}