using AutoMapper;
using priceapp.proxy.Models;
using priceapp.proxy.Repositories.Interfaces;
using priceapp.proxy.Repositories.Models;
using priceapp.proxy.Services.Interfaces;

namespace priceapp.proxy.Services.Implementation;

public class FilialsService : IFilialsService
{
    private readonly IMapper _mapper;
    private readonly IFilialsRepository _filialsRepository;

    public FilialsService(IMapper mapper, IFilialsRepository filialsRepository)
    {
        _mapper = mapper;
        _filialsRepository = filialsRepository;
    }

    public async Task<List<AtbFilialModel>> GetAtbFilialsAsync()
    {
        return _mapper.Map<List<AtbFilialModel>>(await _filialsRepository.GetAtbFilialsAsync());
    }

    public async Task InsertAsync(List<AtbFilialModel> models)
    {
        await _filialsRepository.InsertOrUpdateAsync(_mapper.Map<List<AtbFilialRepositoryModel>>(models));
    }
}