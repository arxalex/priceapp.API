using priceapp.Background.BackgroundTasks;
using priceapp.ControllersLogic;

namespace priceapp.Background;

public static class ServiceCollection
{
    public static void RegisterServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.RegisterBackgroundApiServices(configuration);
        services.RegisterControllersLogicServices(configuration);
    }

    private static void RegisterBackgroundApiServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHostedService<PricesBackgroundTasks>();

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

    }
}