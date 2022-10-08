using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using priceapp.proxy.Repositories.Implementation;
using priceapp.proxy.Repositories.Interfaces;

namespace priceapp.proxy.Repositories;

public static class ServiceCollection
{
    public static void RegisterProxyRepositoriesServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new MySQLDbConnectionFactory(configuration["ConnectionStrings:Proxy"]));

        services.AddScoped<ICategoriesRepository, CategoriesRepository>();
        services.AddSingleton<IConstantsRepository, ConstantsRepository>();
        services.AddScoped<IFilialsRepository, FilialsRepository>();
        services.AddScoped<IItemsRepository, ItemsRepository>();
        services.AddScoped<IPricesRepository, PricesRepository>();
    }
}