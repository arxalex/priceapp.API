using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using priceapp.Repositories;
using priceapp.Services.Implementation;
using priceapp.Services.Interfaces;
using priceapp.Utils;

namespace priceapp.Services;

public static class ServiceCollection
{
    public static void RegisterServicesServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new JWTSetting
        {
            SecretKey = configuration["JWTSetting:SecretKey"],
            Audience = configuration["JWTSetting:Audience"],
            Issuer = configuration["JWTSetting:Issuer"],
            Lifetime = int.Parse(configuration["JWTSetting:Lifetime"])
        });
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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
        
        services.RegisterRepositoryServices(configuration);
    }
}