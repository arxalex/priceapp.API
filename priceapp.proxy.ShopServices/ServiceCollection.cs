using Microsoft.Extensions.DependencyInjection;
using priceapp.proxy.ShopServices.Implementation;
using priceapp.proxy.ShopServices.Interfaces;

namespace priceapp.proxy.ShopServices;

public static class ServiceCollection
{
    public static void RegisterProxyShopServicesServices(this IServiceCollection services)
    {
        services.AddScoped<IAtbService, AtbService>();
    }
}