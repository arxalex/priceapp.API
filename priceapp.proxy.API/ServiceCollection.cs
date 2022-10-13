using priceapp.proxy.API.BackgroundTasks;

namespace priceapp.proxy.API;

public static class ServiceCollection
{
    public static void RegisterProxyApiServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.RegisterProxyApiApiServices();
        services.RegisterProxyServices(configuration);
    }

    private static void RegisterProxyApiApiServices(this IServiceCollection services)
    {
        services.AddHostedService<PricesBackgroundTasks>();

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }
}