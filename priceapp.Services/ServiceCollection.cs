using Microsoft.Extensions.DependencyInjection;
using priceapp.Services.Implementation;
using priceapp.Services.Interfaces;

namespace priceapp.Services;

public static class ServiceCollection
{
    public static void RegisterServicesServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MapperProfile));

        services.AddScoped<IBrandsService, BrandsService>();
        services.AddScoped<ICategoriesService, CategoriesService>();
        services.AddScoped<ICategoryLinksService, CategoryLinksService>();
        services.AddScoped<IConsistsService, ConsistsService>();
        services.AddScoped<ICountriesService, CountriesService>();
        services.AddScoped<IFilialsService, FilialsService>();
        services.AddScoped<IItemLinksService, ItemLinksService>();
        services.AddScoped<IItemsService, ItemsService>();
        services.AddScoped<IMailService, MailService>();
        services.AddScoped<IPackagesService, PackagesService>();
        services.AddScoped<IPricesService, PricesService>();
        services.AddScoped<IShopsService, ShopsService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUsersService, UsersService>();
    }
}