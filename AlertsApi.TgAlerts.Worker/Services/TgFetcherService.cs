using System.Text;
using AlertsApi.Domain.Entities;
using AlertsApi.Domain.Options;
using AlertsApi.Domain.Repositories;
using AlertsApi.TgAlertsFramework;
using AlertsApi.TgAlertsFramework.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlertsApi.TgAlerts.Worker.Services;

public class TgFetcherService : BackgroundService
{
    private const string ChannelName = "UkraineAlarmSignal";
    private const string TestChannelName = "testalertstest";

    private readonly IAlertRepository _alertRepository;
    private readonly ILogger _logger;
    private readonly TgAlarmParser _tgAlarmParser;

    public TgFetcherService(IAlertRepository alertRepository, IOptions<TgAlarmOptions> options, ILogger<TgFetcherService> logger)
    {
        ArgumentNullException.ThrowIfNull(alertRepository, nameof(alertRepository));
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        var tgOptions = options.Value;

        _alertRepository = alertRepository;
        _logger = logger;

        try
        {
            TgAlarmParser.Log = (_, message) => _logger.LogInformation(message);
            _tgAlarmParser =
                new TgAlarmParser(tgOptions.ChannelName!, tgOptions.PhoneNumber!, tgOptions.SessionStorePath!);
        }
        catch (Exception)
        {
            logger.LogError("Failed to initialize telegram client! Channel: {Channel}, Number: {Phone}, StorePath: {Store}",
                tgOptions.ChannelName, tgOptions.PhoneNumber, tgOptions.SessionStorePath);
            throw;
        }
        
        Console.OutputEncoding = Encoding.UTF8;
    }

    private async Task OnUpdates(IEnumerable<TgAlert> obj)
    {
        foreach (var tgAlert in obj)
        {
            await _alertRepository.UpdateAlertAsync(new Alert
                { LocationName = tgAlert.LocationTitle, Active = tgAlert.Active, UpdateTime = tgAlert.FetchedAt });

            var dbAlert = await _alertRepository.GetAlertByLocationAsync(tgAlert.LocationTitle!);
            if (dbAlert is not null)
            {
                await _alertRepository.UpdateAlertAsync(new Alert
                    { LocationName = tgAlert.LocationTitle, Active = tgAlert.Active, UpdateTime = tgAlert.FetchedAt });
            }
            else
            {
                await _alertRepository.CreateAlertAsync(new Alert
                    { LocationName = tgAlert.LocationTitle, Active = tgAlert.Active, UpdateTime = tgAlert.FetchedAt });
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var history = await _tgAlarmParser.GetHistoryAsync(TimeSpan.FromDays(2));
        foreach (var item in history)
        {
            var duration = DateTime.Now - item.FetchedAt;
            if (duration > TimeSpan.FromHours(3))
                item.Active = false;

            var dbAlert = await _alertRepository.GetAlertByLocationAsync(item.LocationTitle!);
            if (dbAlert is null || dbAlert.UpdateTime < item.FetchedAt)
            {
                await _alertRepository.UpdateAlertAsync(new Alert
                    { LocationName = item.LocationTitle, Active = item.Active, UpdateTime = item.FetchedAt });
            }
        }

        _tgAlarmParser.OnUpdates += async obj => await OnUpdates(obj);

        Console.ReadLine();
    }
}