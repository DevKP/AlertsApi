using AlertsApi.Domain.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AlertsApi.TgAlerts.Worker.Services;

public class NotificationsService : BackgroundService
{
    private readonly IAlertRepository _alertRepository;
    private readonly ITelegramBotService _botService;
    private readonly ILogger _logger;

    public NotificationsService(IAlertRepository alertRepository, ITelegramBotService botService,
                                 ILogger<NotificationsService> logger)
    {
        _alertRepository = alertRepository;
        _botService = botService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting telegram bot notifier.");
        _botService.Start();

        await MonitorAlertsStateChangesAsync(stoppingToken);
    }

    private async Task MonitorAlertsStateChangesAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var alertsToNotify = await _alertRepository.GetNotNotifiedAsync();

            foreach (var alert in alertsToNotify)
            {
                _logger.LogInformation($"Sending notifications about {alert.LocationHashTag}");
                await _botService.Notify(alert.LocationHashTag!, alert);

                alert.UsersNotified = true;
                await _alertRepository.UpdateAlertAsync(alert);
            }

            await Task.Delay(6000, stoppingToken);
        }
    }
}