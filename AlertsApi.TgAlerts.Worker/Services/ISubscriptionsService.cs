using AlertsApi.Domain.Entities;

namespace AlertsApi.TgAlerts.Worker.Services;

public interface ISubscriptionsService
{
    Task SubscribeUserAsync(long userId, string locationHashTag);
    Task<Subscription?> GetSubscriptionAsync(long userId, string hashTag);
    Task<IEnumerable<Subscription>> GetSubscriptionsByHashTagAsync(string hashTag);
    Task<bool> IsSubscriptionExistAsync(long userId, string locationHashTag);
    Task UnsubscribeUserAsync(long userId, string locationHashTag);
    Task RemoveAllUserSubscriptionsAsync(long userId);
}