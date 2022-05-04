using AlertsApi.Domain.Entities;

namespace AlertsApi.Domain.Repositories;

public interface ISubscriptionsRepository
{
    Task<IEnumerable<Subscription>> GetAllAsync(int? limit = null);
    Task<Subscription?> GetAsync(long userId, string hashTag);
    Task<Subscription?> GetByIdAsync(int id);
    Task AddAsync(Subscription subscription);
    Task RemoveAsync(Subscription subscription);
    Task RemoveAllByUserAsync(long userId);
    Task<IEnumerable<Subscription>> GetByHashTagAsync(string hashTag);
}