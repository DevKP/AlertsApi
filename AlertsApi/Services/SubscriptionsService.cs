using AlertsApi.Domain.Entities;
using AlertsApi.Domain.Repositories;

namespace AlertsApi.Api.Services;

class SubscriptionsService : ISubscriptionsService
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;

    public SubscriptionsService(ISubscriptionsRepository subscriptionsRepository)
    {
        _subscriptionsRepository = subscriptionsRepository;
    }

    public async Task<IEnumerable<Subscription>> GetAllAsync()
    {
        return await _subscriptionsRepository.GetAllAsync();
    }
}