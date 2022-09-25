namespace priceapp.tasks;

public static class ServiceCollection
{
    public static void RegisterTasksServices(this IServiceCollection services)
    {
        services.AddSingleton<ThreadsUtil>();
    }
}