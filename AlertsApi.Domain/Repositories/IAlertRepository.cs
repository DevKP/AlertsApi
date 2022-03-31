using AlertsApi.Domain.Entities;

namespace AlertsApi.Domain.Repositories;

public interface IAlertRepository
{
    Task CreateAlertAsync(Alert alert);
    Task<IEnumerable<Alert>> GetAllAlertsAsync();
    Task<IEnumerable<Alert>> GetOnlyActiveAsync();
    Task<Alert?> GetAlertAsync(int id);
    Task<Alert?> GetAlertByLocationAsync(string location);
    Task UpdateAlertAsync(Alert alert);
    Task DeleteAlertAsync(string location);
    Task DeleteAlertByLocation(string location);
    Task<bool> IsAlertExits(string location);
}