using priceapp.proxy.Repositories.Interfaces;
using priceapp.proxy.Services.Interfaces;

namespace priceapp.proxy.Services.Implementation;

public class SystemService : ISystemService
{
    private readonly ISystemRepository _systemRepository;

    public SystemService(ISystemRepository systemRepository)
    {
        _systemRepository = systemRepository;
    }

    public async Task<bool> IsDbConnected()
    {
        return await _systemRepository.IsDbConnected();
    }
}