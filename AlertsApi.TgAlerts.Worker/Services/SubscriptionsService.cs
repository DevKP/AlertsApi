using AlertsApi.Domain.Entities;
using AlertsApi.Domain.Repositories;

namespace AlertsApi.TgAlerts.Worker.Services;

public class SubscriptionsService : ISubscriptionsService
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;

    public SubscriptionsService(ISubscriptionsRepository subscriptionsRepository)
    {
        _subscriptionsRepository = subscriptionsRepository;
    }

    public async Task SubscribeUserAsync(long userId, string locationHashTag)
    {
        await _subscriptionsRepository.AddAsync(new Subscription() { UserId = userId, AlertHashTag = locationHashTag });
    }

    public async Task<Subscription?> GetSubscriptionAsync(long userId, string hashTag)
    {
        return await _subscriptionsRepository.GetAsync(userId, hashTag);
    }

    public async Task<IEnumerable<Subscription>> GetSubscriptionsByHashTagAsync(string hashTag)
    {
        return await _subscriptionsRepository.GetByHashTagAsync(hashTag);
    }

    public async Task<bool> IsSubscriptionExistAsync(long userId, string locationHashTag)
    {
        var subscription = await _subscriptionsRepository.GetAsync(userId, locationHashTag);
        return subscription is not null;
    }

    public async Task UnsubscribeUserAsync(long userId, string locationHashTag)
    {
        var subscription = await _subscriptionsRepository.GetAsync(userId, locationHashTag);
        if (subscription is not null)
        {
            await _subscriptionsRepository.RemoveAsync(subscription);
        }
    }

    public async Task RemoveAllUserSubscriptionsAsync(long userId)
    {
        await _subscriptionsRepository.RemoveAllByUserAsync(userId);
    }
}