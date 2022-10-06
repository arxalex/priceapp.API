using AutoMapper;
using priceapp.Models;
using priceapp.Repositories.Interfaces;
using priceapp.Repositories.Models;
using priceapp.Services.Interfaces;

namespace priceapp.Services.Implementation;

public class FilialsService : IFilialsService
{
    private readonly IFilialsRepository _filialsRepository;
    private readonly IMapper _mapper;

    public FilialsService(IMapper mapper, IFilialsRepository filialsRepository)
    {
        _mapper = mapper;
        _filialsRepository = filialsRepository;
    }

    public async Task<List<FilialModel>> GetFilialsAsync(double xCord, double yCord, double radius)
    {
        return _mapper.Map<List<FilialModel>>(await _filialsRepository.GetFilialsAsync(xCord, yCord, radius));
    }

    public async Task<List<FilialModel>> GetFilialsAsync()
    {
        return _mapper.Map<List<FilialModel>>(await _filialsRepository.GetFilialsAsync());
    }

    public async Task<string> GetRegionAsync(string city)
    {
        return await _filialsRepository.GetRegionAsync(city);
    }

    public async Task InsertFilialsAsync(List<FilialModel> filialsToInsert)
    {
        await _filialsRepository.InsertFilialsAsync(_mapper.Map<List<FilialRepositoryModel>>(filialsToInsert));
    }
}