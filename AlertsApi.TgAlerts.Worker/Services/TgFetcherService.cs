using System.Text;
using AlertsApi.Domain.Entities;
using AlertsApi.Domain.Repositories;
using AlertsApi.TgAlertsFramework;
using AlertsApi.TgAlertsFramework.Models;
using Microsoft.Extensions.Hosting;

namespace AlertsApi.TgAlerts.Worker.Services;

public class TgFetcherService : BackgroundService
{
    private const string ChannelName = "UkraineAlarmSignal";
    private const string TestChannelName = "testalertstest";

    private readonly IAlertRepository _alertRepository;
    private readonly TgAlarmParser _tgAlarmParser;

    public TgFetcherService(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
        _tgAlarmParser = new TgAlarmParser(ChannelName);
        _tgAlarmParser.OnUpdates += OnUpdates;

        Console.OutputEncoding = Encoding.UTF8;
    }

    private async void OnUpdates(IEnumerable<TgAlert> obj)
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
        var history = _tgAlarmParser.GetHistoryAsync(TimeSpan.FromDays(1));
        await foreach (var item in history)
        {
            var duration = DateTime.Now - item.FetchedAt;
            if (duration > TimeSpan.FromHours(2))
                item.Active = false;

            var dbAlert = await _alertRepository.GetAlertByLocationAsync(item.LocationTitle!);
            if (dbAlert is not null && dbAlert.UpdateTime < item.FetchedAt)
            {
                await _alertRepository.UpdateAlertAsync(new Alert
                    { LocationName = item.LocationTitle, Active = item.Active, UpdateTime = item.FetchedAt });
            }
        }

        Console.ReadLine();
    }
}