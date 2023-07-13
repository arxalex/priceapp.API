using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using priceapp.proxy.Repositories;
using priceapp.proxy.Services.Implementation;
using priceapp.proxy.Services.Interfaces;

namespace priceapp.proxy.Services;

public static class ServiceCollection
{
    public static void RegisterProxyServicesServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(MapperProfile));

        services.AddScoped<ICategoriesService, CategoriesService>();
        services.AddSingleton<IConstantsService, ConstantsService>();
        services.AddScoped<IFilialsService, FilialsService>();
        services.AddScoped<IItemsService, ItemsService>();
        services.AddScoped<IPricesService, PricesService>();
        services.AddScoped<ISystemService, SystemService>();

        services.RegisterProxyRepositoriesServices(configuration);
    }
}