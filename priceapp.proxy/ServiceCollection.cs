using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using priceapp.proxy.Controllers;
using priceapp.proxy.Controllers.Logic;
using priceapp.proxy.Services;
using priceapp.proxy.ShopServices;
using priceapp.tasks;

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
        services.AddScoped<PricesControllerLogic>();

        services.RegisterProxyServicesServices(configuration);
        services.RegisterProxyShopServicesServices();
        services.RegisterTasksServices();
    }
}