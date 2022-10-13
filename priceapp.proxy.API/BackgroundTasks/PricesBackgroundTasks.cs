using priceapp.proxy.Controllers;

namespace priceapp.proxy.API.BackgroundTasks;

public class PricesBackgroundTasks : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PricesBackgroundTasks> _logger;
    private const int ShopId = 3;
    private const int Hour = 4;

    public PricesBackgroundTasks(IServiceProvider serviceProvider, ILogger<PricesBackgroundTasks> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.Log(LogLevel.Information, "Price actualizing background task started");
        while (!stoppingToken.IsCancellationRequested)
        {
            var date = DateTime.Now.Date;
            _logger.Log(LogLevel.Information, "Price actualizing started at {Now}", DateTime.Now);

            using (var scope = _serviceProvider.CreateScope())
            {
                var pricesController = scope.ServiceProvider.GetRequiredService<PricesController>();
                await pricesController.ActualizeProxyPricesAsync(ShopId);
            }

            _logger.Log(LogLevel.Information, "Price actualizing finished at {Now}", DateTime.Now);

            if (DateTime.Now.Date <= date)
            {
                await Task.Delay(DateTime.Now.Date.AddDays(1).AddHours(Hour) - DateTime.Now, stoppingToken);
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.Log(LogLevel.Information, "Price actualizing background task stopped at {Now}", DateTime.Now);

        return base.StopAsync(cancellationToken);
    }
}