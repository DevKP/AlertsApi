using AlertsApi.Domain.Entities;
using AlertsApi.Domain.Queries;

namespace AlertsApi.Domain.Repositories;

public interface IAlertRepository
{
    Task CreateAlertAsync(Alert alert);
    Task<IEnumerable<Alert>> GetAllAlertsAsync();
    Task<IEnumerable<Alert>> GetOnlyActiveAsync();
    Task<IEnumerable<Alert>> GetNotNotifiedAsync();
    Task<IEnumerable<Alert>> GetQueryAsync(AlertsQuery query);
    Task<Alert?> GetAlertAsync(int id);
    Task<Alert?> GetAlertByHashTagAsync(string hashTag);
    Task UpdateAlertAsync(Alert alert);
    Task DeleteAlertAsync(string hashTag);
    Task DeleteAlertByLocation(string hashTag);
    Task<bool> IsAlertExits(string hashTag);
}