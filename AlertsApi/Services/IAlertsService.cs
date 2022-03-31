using AlertsApi.Api.Models.Responses;

namespace AlertsApi.Api.Services;

public interface IAlertsService
{
    Task<AlertResponse?> GetAlertById(int id);
    Task<AlertsResponse> GetAllAlerts();
    Task<AlertsResponse> GetActiveAlerts();
}