using AlertsApi.Domain.Entities;

namespace AlertsApi.Api.Services;

public interface ISubscriptionsService
{
    Task<IEnumerable<Subscription>> GetAllAsync();
}