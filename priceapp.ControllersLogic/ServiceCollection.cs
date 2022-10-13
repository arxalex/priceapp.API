using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using priceapp.proxy;
using priceapp.Services;
using priceapp.ShopsServices;

namespace priceapp.ControllersLogic;

public static class ServiceCollection
{
    public static void RegisterControllersLogicServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<SessionParameters>();
        services.AddScoped<PricesControllerUpdateLogic>();
        services.RegisterProxyServices(configuration);
        services.RegisterServicesServices(configuration);
        services.RegisterShopServicesServices();
    }
}