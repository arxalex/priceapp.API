using AutoMapper;
using priceapp.API.Models;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Services.Interfaces;

namespace priceapp.API.Services.Implementation;

public class FilialsService : IFilialsService
{
    private readonly IFilialsRepository _filialsRepository;
    private readonly IMapper _mapper;

    public FilialsService(IMapper mapper, IFilialsRepository filialsRepository)
    {
        _mapper = mapper;
        _filialsRepository = filialsRepository;
    }

    public async Task<List<FilialModel>> GetFilialsByLocationAsync(double xCord, double yCord, double radius)
    {
        return _mapper.Map<List<FilialModel>>(await _filialsRepository.GetFilialsByLocationAsync(xCord, yCord, radius));
    }
}