using AutoMapper;
using priceapp.API.Models;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Repositories.Models;
using priceapp.API.Services.Interfaces;
using priceapp.API.ShopServices.Interfaces;

namespace priceapp.API.Services.Implementation;

public class FilialsService : IFilialsService
{
    private readonly IFilialsRepository _filialsRepository;
    private readonly IMapper _mapper;
    private readonly ISilpoService _silpoService;
    private readonly IForaService _foraService;
    private readonly IAtbService _atbService;
    private readonly IShopsService _shopsService;

    public FilialsService(IMapper mapper, IFilialsRepository filialsRepository, ISilpoService silpoService,
        IForaService foraService, IAtbService atbService, IShopsService shopsService)
    {
        _mapper = mapper;
        _filialsRepository = filialsRepository;
        _silpoService = silpoService;
        _foraService = foraService;
        _atbService = atbService;
        _shopsService = shopsService;
    }

    public async Task<List<FilialModel>> GetFilialsAsync(double xCord, double yCord, double radius)
    {
        return _mapper.Map<List<FilialModel>>(await _filialsRepository.GetFilialsAsync(xCord, yCord, radius));
    }

    public async Task<List<FilialModel>> GetFilialsAsync()
    {
        return _mapper.Map<List<FilialModel>>(await _filialsRepository.GetFilialsAsync());
    }

    public async Task<List<FilialModel>> GetFilialsAsync(int shopId)
    {
        var filials = shopId switch
        {
            1 => await _silpoService.GetFilialsAsync(),
            2 => await _foraService.GetFilialsAsync(),
            3 => await _atbService.GetFilialsAsync(),
            _ => new List<FilialModel>()
        };

        return filials;
    }

    public async Task ActualizeFilialsAsync()
    {
        var shops = await _shopsService.GetShopsAsync();
        var filials = new List<FilialModel>();
        var filialsInserted = await GetFilialsAsync();
        foreach (var shop in shops)
        {
            filials.AddRange(await GetFilialsAsync(shop.Id));
        }

        var filialsToInsert = filials
            .Where(x => filialsInserted.Count(y => y.InShopId == x.InShopId) < 1)
            .ToList();

        await _filialsRepository.InsertFilialsAsync(_mapper.Map<List<FilialRepositoryModel>>(filialsToInsert));
    }

    public async Task<string> GetRegionAsync(string city)
    {
        return await _filialsRepository.GetRegionAsync(city);
    }
}