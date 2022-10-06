using Microsoft.Extensions.DependencyInjection;
using priceapp.ShopsServices.Implementation;
using priceapp.ShopsServices.Interfaces;

namespace priceapp.ShopsServices;

public static class ServiceCollection
{
    public static void RegisterShopServicesServices(this IServiceCollection services)
    {
        services.AddScoped<ISilpoService, SilpoService>();
        services.AddScoped<IAtbService, AtbService>();
        services.AddScoped<IForaService, ForaService>();
    }
}