using Microsoft.Extensions.Configuration;
using priceapp.proxy.Models;
using priceapp.proxy.Services.Interfaces;

namespace priceapp.proxy;

public class SessionParameters
{
    private readonly IConfiguration _configuration;
    private readonly IConstantsService _constantsService;
    private bool _isActualizeProxyAtbPricesActive;

    public SessionParameters(IConfiguration configuration, IConstantsService constantsService)
    {
        _configuration = configuration;
        _constantsService = constantsService;
        IsActualizeProxyAtbPricesActive = false;
    }

    public bool IsActualizeProxyAtbPricesActive
    {
        get => !bool.Parse(_configuration["Proxy:MultiInstance"])
            ? _isActualizeProxyAtbPricesActive
            : bool.Parse(_constantsService.GetConstantAsync("ACTUALIZE_PROXY_ATB_PRICES_ACTIVE").Result.Value);
        set
        {
            if (bool.Parse(_configuration["Proxy:MultiInstance"]))
            {
                _constantsService.SetConstantAsync(new ConstantModel
                {
                    Id = -1,
                    Label = "ACTUALIZE_PROXY_ATB_PRICES_ACTIVE",
                    Value = $"{value}"
                }).Wait();
            }
            else
            {
                _isActualizeProxyAtbPricesActive = value;
            }
        }
    }
}