using AlertsApi.Api.Models;
using AlertsApi.Api.Models.Responses;
using AlertsApi.Domain.Queries;
using AlertsApi.Domain.Repositories;
using AutoMapper;

namespace AlertsApi.Api.Services;

public class AlertsService : IAlertsService
{
    private readonly IAlertRepository _alertRepository;
    private readonly IMapper _mapper;

    public AlertsService(IAlertRepository alertRepository, IMapper mapper)
    {
        _alertRepository = alertRepository;
        _mapper = mapper;
    }

    public async Task<AlertResponse?> GetAlertById(int id)
    {
        var alert = await _alertRepository.GetAlertAsync(id);
        if (alert is null)
            return null;

        var alertResponse = _mapper.Map<AlertResponse>(alert);
        return alertResponse;
    }

    public async Task<AlertsResponse> Get(AlertsQuery query)
    {
        var alerts = await _alertRepository.GetQueryAsync(query);

        var alertsResponse = _mapper.Map<AlertsResponse>(alerts);
        return alertsResponse;
    }

    public async Task<AlertsResponse> GetAllAlerts()
    {
        var alerts = await _alertRepository.GetAllAlertsAsync();

        var alertsResponse = _mapper.Map<AlertsResponse>(alerts);
        return alertsResponse;
    }

    public async Task<AlertsResponse> GetActiveAlerts()
    {
        var alerts = await _alertRepository.GetOnlyActiveAsync();

        var alertsResponse = _mapper.Map<AlertsResponse>(alerts);
        return alertsResponse;
    }
}