using AutoMapper;
using priceapp.Models;
using priceapp.Repositories.Interfaces;
using priceapp.Repositories.Models;
using priceapp.Services.Interfaces;

namespace priceapp.Services.Implementation;

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

    public async Task InsertConsistAsync(ConsistModel model)
    {
        await _consistsRepository.InsertConsistAsync(_mapper.Map<ConsistRepositoryModel>(model));
    }

    public async Task UpdateConsistAsync(ConsistModel model)
    {
        await _consistsRepository.UpdateConsistAsync(_mapper.Map<ConsistRepositoryModel>(model));
    }
}