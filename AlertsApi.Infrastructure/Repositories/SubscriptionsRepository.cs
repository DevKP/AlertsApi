using AlertsApi.Domain.Entities;
using AlertsApi.Domain.Repositories;
using AlertsApi.Infrastructure.Db;
using AlertsApi.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AlertsApi.Infrastructure.Repositories;

public class SubscriptionsRepository : ISubscriptionsRepository
{
    private readonly AlertDbContext _alertDbContext;

    public SubscriptionsRepository(AlertDbContext alertDbContext)
    {
        _alertDbContext = alertDbContext;
    }

    public async Task<IEnumerable<Subscription>> GetAllAsync(int? limit = null)
    {
        return await _alertDbContext.Subscriptions!.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Subscription>> GetAllByUserAsync(long userId)
    {
        return await _alertDbContext.Subscriptions!.AsNoTracking().Where(sub => sub.UserId == userId).ToListAsync();
    }

    public async Task<Subscription?> GetAsync(long userId, string hashTag)
    {
        return await _alertDbContext.Subscriptions!.AsNoTracking()
            .FirstOrDefaultAsync(sub => sub.UserId == userId && sub.AlertHashTag == hashTag);
    }

    public async Task<Subscription?> GetByIdAsync(int id)
    {
        return await _alertDbContext.Subscriptions!.AsNoTracking().FirstOrDefaultAsync(sub => sub.Id == id);
    }

    public async Task AddAsync(Subscription subscription)
    {
        await _alertDbContext.Subscriptions!.AddAsync(subscription);
        await _alertDbContext.SaveChangesAsync();
        
        _alertDbContext.DetachEntry(subscription);
    }

    public async Task RemoveAsync(Subscription subscription)
    {
        _alertDbContext.Subscriptions!.Remove(subscription);
        await _alertDbContext.SaveChangesAsync();
    }

    public async Task RemoveAllByUserAsync(long userId)
    {
        var userSubscriptions = _alertDbContext.Subscriptions!.Where(sub => sub.UserId == userId);
        _alertDbContext.Subscriptions!.RemoveRange(userSubscriptions);
        await _alertDbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Subscription>> GetByHashTagAsync(string hashTag)
    {
        return await _alertDbContext.Subscriptions!.AsNoTracking().Where(sub => sub.AlertHashTag == hashTag).ToListAsync();
    }
}