using priceapp.proxy.Controllers;
using priceapp.proxy.Repositories;
using priceapp.proxy.Repositories.Implementation;
using priceapp.proxy.Repositories.Interfaces;
using priceapp.proxy.Services.Implementation;
using priceapp.proxy.Services.Interfaces;
using priceapp.proxy.ShopServices.Implementation;
using priceapp.proxy.ShopServices.Interfaces;
using priceapp.proxy.Utils;

namespace priceapp.proxy;

public static class ServiceCollection
{
    public static void RegisterProxyServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<CategoriesController>();
        services.AddScoped<FilialsController>();
        services.AddScoped<ItemsController>();
        services.AddScoped<PricesController>();
        
        services.AddAutoMapper(typeof(MapperProfile));
        services.AddSingleton(new MySQLDbConnectionFactory(configuration["ConnectionStrings:Proxy"]));

        services.AddSingleton<IAtbService, AtbService>();

        services.AddSingleton<ICategoriesService, CategoriesService>();
        services.AddSingleton<IFilialsService, FilialsService>();
        services.AddSingleton<IItemsService, ItemsService>();
        services.AddSingleton<IPricesService, PricesService>();

        services.AddSingleton<ICategoriesRepository, CategoriesRepository>();
        services.AddSingleton<IFilialsRepository, FilialsRepository>();
        services.AddSingleton<IItemsRepository, ItemsRepository>();
        services.AddSingleton<IPricesRepository, PricesRepository>();
    }
}