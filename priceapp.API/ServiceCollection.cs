using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using priceapp.API.Repositories;
using priceapp.API.Repositories.Implementation;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Services.Implementation;
using priceapp.API.Services.Interfaces;
using priceapp.API.ShopServices.Implementation;
using priceapp.API.ShopServices.Interfaces;
using priceapp.API.Utils;
using priceapp.proxy;
using priceapp.tasks;

namespace priceapp.API;

public static class ServiceCollection
{
    public static void RegisterServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            var secretKey = configuration["JWTSetting:SecretKey"];
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var serviceProvider = services.BuildServiceProvider();
                    var tokenService = serviceProvider.GetService<ITokenService>();
                    if (!tokenService!.IsCurrentTokenActive().Result) context.Fail("Unauthorized");

                    return Task.CompletedTask;
                }
            };
            options.RequireHttpsMetadata = true;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddAutoMapper(typeof(MapperProfile));
        services.AddSingleton(new MySQLDbConnectionFactory(configuration["ConnectionStrings:Default"]));
        services.AddSingleton(new JWTSetting
        {
            SecretKey = configuration["JWTSetting:SecretKey"],
            Audience = configuration["JWTSetting:Audience"],
            Issuer = configuration["JWTSetting:Issuer"],
            Lifetime = int.Parse(configuration["JWTSetting:Lifetime"])
        });
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<SessionParameters>();

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

        services.AddScoped<ISilpoService, SilpoService>();
        services.AddScoped<IAtbService, AtbService>();
        services.AddScoped<IForaService, ForaService>();

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

        services.AddCors(options =>
        {
            options.AddPolicy(
                "CorsPolicy",
                corsPolicyBuilder => corsPolicyBuilder.WithOrigins(configuration["Cors:Url"])
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });
        
        services.RegisterProxyServices(configuration);
        services.RegisterTasksServices();
    }
}