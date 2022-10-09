using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using priceapp.proxy.Services;
using priceapp.proxy.ShopServices;
using priceapp.Services;
using priceapp.Services.Interfaces;
using priceapp.Utils;

namespace priceapp.proxy.API;

public static class ServiceCollection
{
    public static void RegisterProxyApiServices(this IServiceCollection services,
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
        services.AddSingleton(new JWTSetting
        {
            SecretKey = configuration["JWTSetting:SecretKey"],
            Audience = configuration["JWTSetting:Audience"],
            Issuer = configuration["JWTSetting:Issuer"],
            Lifetime = int.Parse(configuration["JWTSetting:Lifetime"])
        });
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<SessionParameters>();

        services.AddCors(options =>
        {
            options.AddPolicy(
                "CorsPolicy",
                corsPolicyBuilder => corsPolicyBuilder.WithOrigins(configuration["Cors:Url"],
                        configuration["Cors:UrlSSL"],
                        configuration["Cors:UrlLocal"])
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });

        services.RegisterProxyServicesServices(configuration);
        services.RegisterProxyShopServicesServices();
        services.RegisterServicesServices(configuration);
    }
}