using AutoMapper;
using priceapp.proxy.Models;
using priceapp.proxy.Repositories.Interfaces;
using priceapp.proxy.Repositories.Models;
using priceapp.proxy.Services.Interfaces;

namespace priceapp.proxy.Services.Implementation;

public class ConstantsService : IConstantsService
{
    private readonly IConstantsRepository _constantsRepository;
    private readonly IMapper _mapper;

    public ConstantsService(IConstantsRepository constantsRepository, IMapper mapper)
    {
        _constantsRepository = constantsRepository;
        _mapper = mapper;
    }

    public async Task<ConstantModel> GetConstantAsync(string label)
    {
        return _mapper.Map<ConstantModel>(await _constantsRepository.GetConstantAsync(label));
    }

    public async Task SetConstantAsync(ConstantModel model)
    {
        await _constantsRepository.InsertOrUpdateConstantAsync(_mapper.Map<ConstantRepositoryModel>(model));
    }
}