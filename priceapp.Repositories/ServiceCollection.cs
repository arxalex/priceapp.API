using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using priceapp.Repositories.Implementation;
using priceapp.Repositories.Interfaces;

namespace priceapp.Repositories;

public static class ServiceCollection
{
    public static void RegisterRepositoryServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new MySQLDbConnectionFactory(configuration["ConnectionStrings:Default"]));

        services.AddScoped<IBrandsRepository, BrandsRepository>();
        services.AddScoped<ICategoriesRepository, CategoriesRepository>();
        services.AddScoped<ICategoryLinksRepository, CategoryLinksRepository>();
        services.AddScoped<IConsistsRepository, ConsistsRepository>();
        services.AddScoped<ICountriesRepository, CountriesRepository>();
        services.AddScoped<IFilialsRepository, FilialsRepository>();
        services.AddScoped<IItemsRepository, ItemsRepository>();
        services.AddScoped<IPackagesRepository, PackagesRepository>();
        services.AddScoped<IPricesRepository, PricesRepository>();
        services.AddScoped<IShopsRepository, ShopsRepository>();
        services.AddScoped<ITokensRepository, TokensRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
    }
}