using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using priceapp.proxy.Controllers;
using priceapp.proxy.Services;
using priceapp.proxy.ShopServices;

namespace priceapp.proxy;

public static class ServiceCollection
{
    public static void RegisterProxyServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<SessionParameters>();

        services.AddScoped<CategoriesController>();
        services.AddScoped<FilialsController>();
        services.AddScoped<ItemsController>();
        services.AddScoped<PricesController>();

        services.RegisterProxyServicesServices(configuration);
        services.RegisterProxyShopServicesServices();
    }
}