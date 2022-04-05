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
            // If alert doesn't exist, create it
            var dbAlert = await _alertRepository.GetAlertByLocationAsync(alert.LocationTitle!);
            if (dbAlert is null)
            {
                var alertEntity = _mapper.Map<TgAlert, Alert>(alert);
                alertEntity.UpdateTime = alert.FetchedAt;
                if(alert.Active)
                    alertEntity.StartTime = alert.FetchedAt;
                else
                    alertEntity.EndTime = alert.FetchedAt;

                await _alertRepository.CreateAlertAsync(alertEntity);

                _logger.LogInformation("New location. Location: {Location}, State: {State}",
                    alertEntity.LocationName, alertEntity.Active);

                continue;
            }

            if (SetAlertState(alert, dbAlert))
            {
                dbAlert.UpdateTime = alert.FetchedAt;

                await _alertRepository.UpdateAlertAsync(dbAlert);

                _logger.LogInformation("Alert state changed. Location: {Location}, State: {State}",
                    dbAlert.LocationName, dbAlert.Active);
            }
        }

    }

    private static bool SetAlertState(TgAlert alert, Alert dbAlert)
    {
        if (alert.Active)
        {
            return ActivateAlert(dbAlert, alert);
        }
        else
        {
            return DeactivateAlert(dbAlert, alert);
        }
    }

    private static bool DeactivateAlert(Alert dbAlert, TgAlert alert)
    {
        dbAlert.Active = false;
        
        if (dbAlert.StartTime is not null &&
            dbAlert.StartTime < alert.FetchedAt)
        {
            dbAlert.EndTime = alert.FetchedAt;
            return true;
        }
        
        return false;
    }
    
    private static bool ActivateAlert(Alert dbAlert, TgAlert alert)
    {
        dbAlert.Active = true;
        
        var changed = false;
        if (dbAlert.StartTime is null ||
            dbAlert.StartTime < alert.FetchedAt)
        {
            dbAlert.StartTime = alert.FetchedAt;
            changed = true;
        }

        if (dbAlert.EndTime is not null &&
            dbAlert.StartTime > dbAlert.EndTime)
        {
            dbAlert.EndTime = null;
            changed = true;
        }

        return changed;
    }
}