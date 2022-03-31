using AlertsApi.TgAlerts.Worker.Models;

namespace AlertsApi.TgAlerts.Worker.Services;

public interface IAlertsService
{
    Task UpdateAlertsAsync(IEnumerable<TgAlert> alerts);
}