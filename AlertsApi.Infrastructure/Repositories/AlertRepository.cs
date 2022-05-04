using AlertsApi.Domain.Entities;
using AlertsApi.Domain.Queries;
using AlertsApi.Domain.Repositories;
using AlertsApi.Infrastructure.Db;
using AlertsApi.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AlertsApi.Infrastructure.Repositories;

public class AlertRepository : IAlertRepository
{
    private readonly AlertDbContext _dbContext;

    public AlertRepository(AlertDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAlertAsync(Alert alert)
    {
        await _dbContext.Alerts!.AddAsync(alert);
        await _dbContext.SaveChangesAsync();
        _dbContext.DetachEntry(alert);
    }

    public async Task<IEnumerable<Alert>> GetAllAlertsAsync()
    {
        return await _dbContext.Alerts!.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Alert>> GetOnlyActiveAsync()
    {
        return await _dbContext.Alerts!.AsNoTracking().Where(a => a.Active).ToListAsync();
    }

    public async Task<IEnumerable<Alert>> GetQueryAsync(AlertsQuery alertQuery)
    {
        var query = _dbContext.Alerts!.AsNoTracking();

        if (alertQuery.From is not null)
        {
            query = query.Where(a => alertQuery.From <= (a.EndTime ?? a.StartTime));
        }

        if (alertQuery.To is not null)
        {
            query = query.Where(a => a.UpdateTime <= alertQuery.To);
        }

        if (alertQuery.Active is not null)
        {
            query = query.Where(a => a.Active == alertQuery.Active);
        }

        return await query.ToListAsync();
    }

    public async Task<Alert?> GetAlertAsync(int id)
    {
        return await _dbContext.Alerts!.AsNoTracking().FirstOrDefaultAsync(alert => alert.Id == id);
    }

    public async Task<Alert?> GetAlertByHashTagAsync(string hashTag)
    {
        return await _dbContext.Alerts!.AsNoTracking().FirstOrDefaultAsync(alert => alert.LocationHashTag == hashTag);
    }

    public async Task UpdateAlertAsync(Alert alert)
    {
        _dbContext.Alerts!.Update(alert);
        await _dbContext.SaveChangesAsync();
        _dbContext.Entry(alert).State = EntityState.Detached;
    }

    public Task DeleteAlertAsync(string hashTag)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAlertByLocation(string hashTag)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsAlertExits(string hashTag)
    {
        return await _dbContext.Alerts!.AnyAsync(a => a.LocationHashTag == hashTag);
    }

    public async Task<IEnumerable<Alert>> GetNotNotifiedAsync()
    {
       return await _dbContext.Alerts!.Where(a => a.UsersNotified == false)
                                        .AsNoTracking().ToListAsync();
    }
}