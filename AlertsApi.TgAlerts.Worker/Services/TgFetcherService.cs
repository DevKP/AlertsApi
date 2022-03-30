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
    private readonly IAlertRepository _alertRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly ILogger _logger;
    private readonly TgAlarmParser _tgAlarmParser;

    public TgFetcherService(IAlertRepository alertRepository, IOptions<TgAlarmOptions> options,
        ILogger<TgFetcherService> logger, IMessageRepository messageRepository)
    {
        ArgumentNullException.ThrowIfNull(alertRepository, nameof(alertRepository));
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        var tgOptions = options.Value;

        _alertRepository = alertRepository;
        _logger = logger;
        _messageRepository = messageRepository;

        try
        {
            TgAlarmParser.Log = (_, message) => _logger.LogDebug(message);
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
            _logger.LogInformation("Status update for location: {Location}. Status: {Status}",
                tgAlert.LocationTitle, tgAlert.Active ? "Active" : "Not Active");

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
        _logger.LogInformation("Start to fetching history from tg channel..");
        var history = await _tgAlarmParser.GetHistoryAsync(TimeSpan.FromDays(1));
        foreach (var item in history)
        {
            var duration = DateTime.Now - item.FetchedAt;
            if (duration > TimeSpan.FromHours(3))
            {
                item.Active = false;
            }
            else
            {
                _logger.LogInformation("Found fresh status from history. Location: {Location}. Status: {Status}",
                    item.LocationTitle, item.Active ? "Active" : "Not Active");
            }

            var dbAlert = await _alertRepository.GetAlertByLocationAsync(item.LocationTitle!);
            if (dbAlert is null || dbAlert.UpdateTime < item.FetchedAt)
            {
                await _alertRepository.UpdateAlertAsync(new Alert
                    { LocationName = item.LocationTitle, Active = item.Active, UpdateTime = item.FetchedAt });
            }
        }

        _logger.LogInformation("Start monitoring real time updates.");
        _tgAlarmParser.OnUpdates += async obj => await OnUpdates(obj);
        _tgAlarmParser.OnNewMessage += async messages =>
        {
            var messagesEntities = messages.Select(m => new MessageEntity()
            {
                Id = m.ID,
                Message = m.message,
                Date = m.Date
            });
            await _messageRepository.InsertRangeAsync(messagesEntities);
        };

        Console.ReadLine();
    }
}