using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using priceapp.ControllersLogic;
using priceapp.Services.Interfaces;

namespace priceapp.API;

public static class ServiceCollection
{
    public static void RegisterServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.RegisterApiServices(configuration);
        services.RegisterControllersLogicServices(configuration);
    }

    private static void RegisterApiServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

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
    }
}