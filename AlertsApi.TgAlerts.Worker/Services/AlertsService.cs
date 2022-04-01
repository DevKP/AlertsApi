using AlertsApi.Domain.Entities;
using AlertsApi.Domain.Repositories;
using AlertsApi.TgAlerts.Worker.Models;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AlertsApi.TgAlerts.Worker.Services;

class AlertsService : IAlertsService
{
    private readonly IAlertRepository _alertRepository;
    private readonly ILogger<AlertsService> _logger;
    private readonly IMapper _mapper;

    public AlertsService(IAlertRepository alertRepository, IMapper mapper, ILogger<AlertsService> logger)
    {
        _alertRepository = alertRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task UpdateAlertsAsync(IEnumerable<TgAlert> alerts)
    {
        foreach (var alert in alerts)
        {
            var dbAlert = await _alertRepository.GetAlertByLocationAsync(alert.LocationTitle!);
            if (dbAlert is null)
            {
                var alertEntity = _mapper.Map<TgAlert, Alert>(alert);
                alertEntity.UpdateTime = alert.FetchedAt;
                SetStartEndTime(alert, alertEntity);

                await _alertRepository.CreateAlertAsync(alertEntity);

                _logger.LogInformation("New location. Location: {Location}, State: {State}",
                    alertEntity.LocationName, alertEntity.Active);

                continue;
            }

            if (alert.FetchedAt > dbAlert.UpdateTime)
            {
                dbAlert.Active = alert.Active;
                dbAlert.UpdateTime = alert.FetchedAt;
                SetStartEndTime(alert, dbAlert);

                await _alertRepository.UpdateAlertAsync(dbAlert);

                _logger.LogInformation("Alert state changed. Location: {Location}, State: {State}",
                    dbAlert.LocationName, dbAlert.Active);
            }
        }
        
    }

    private static void SetStartEndTime(TgAlert alertFromTelegram, Alert alert)
    {
        if (alertFromTelegram.Active)
        {
            alert.StartTime = alertFromTelegram.FetchedAt;
            alert.EndTime = null;
        }
        else
        {
            alert.EndTime = alertFromTelegram.FetchedAt;
        }
    }
}