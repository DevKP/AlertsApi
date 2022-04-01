using AlertsApi.Api.Models;
using AlertsApi.Api.Models.Responses;
using AlertsApi.Domain.Queries;

namespace AlertsApi.Api.Services;

public interface IAlertsService
{
    Task<AlertResponse?> GetAlertById(int id);
    Task<AlertsResponse> Get(AlertsQuery query);
    Task<AlertsResponse> GetAllAlerts();
    Task<AlertsResponse> GetActiveAlerts();
}