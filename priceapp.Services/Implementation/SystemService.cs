using AutoMapper;
using priceapp.Models;
using priceapp.Repositories.Interfaces;
using priceapp.Services.Interfaces;

namespace priceapp.Services.Implementation;

public class SystemService : ISystemService
{
    private readonly ISystemRepository _systemRepository;
    private readonly IVersionsRepository _versionsRepository;
    private readonly IMapper _mapper;

    public SystemService(ISystemRepository systemRepository, IVersionsRepository versionsRepository, IMapper mapper)
    {
        _systemRepository = systemRepository;
        _versionsRepository = versionsRepository;
        _mapper = mapper;
    }

    public async Task<bool> IsDbConnected()
    {
        return await _systemRepository.IsDbConnected();
    }

    public async Task<bool> IsNeedUpdate(string version)
    {
        var versionArray = version.Split('.');
        if (versionArray.Length != 3)
        {
            throw new FormatException("Version string is not valid");
        }

        VersionModel versionObj;
        try
        {
            versionObj = new VersionModel
            {
                Version = int.Parse(versionArray[0]),
                Major = int.Parse(versionArray[1]),
                Minor = int.Parse(versionArray[2])
            };
        }
        catch (Exception)
        {
            throw new FormatException("Version string is not valid");
        }

        var minVersion = _mapper.Map<VersionModel>(await _versionsRepository.GetMinVersion());
        return versionObj < minVersion;
    }
}