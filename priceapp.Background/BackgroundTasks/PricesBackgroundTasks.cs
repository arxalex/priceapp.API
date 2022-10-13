using priceapp.ControllersLogic;

namespace priceapp.Background.BackgroundTasks;

public class PricesBackgroundTasks : BackgroundService
{
    private readonly ILogger<PricesBackgroundTasks> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const int Hour = 4;

    public PricesBackgroundTasks(ILogger<PricesBackgroundTasks> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
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
                var pricesControllerUpdateLogic = scope.ServiceProvider.GetRequiredService<PricesControllerUpdateLogic>();
                await pricesControllerUpdateLogic.ActualizePricesAsync();
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