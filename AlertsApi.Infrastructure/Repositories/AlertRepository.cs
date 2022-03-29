using AlertsApi.Domain.Entities;
using AlertsApi.Domain.Repositories;
using AlertsApi.Infrastructure.Db;
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
    }

    public async Task<IEnumerable<Alert>> GetAllAlertsAsync()
    {
        return await _dbContext.Alerts!.AsNoTracking().ToListAsync();
    }

    public async Task<Alert?> GetAlertAsync(int id)
    {
        return await _dbContext.Alerts!.FindAsync(id);
    }

    public async Task<Alert?> GetAlertByLocationAsync(string location)
    {
        return await _dbContext.Alerts!.FindAsync(location);
    }
    
    public async Task UpdateAlertAsync(Alert alert)
    {
        var alertDb = await _dbContext.Alerts!.FindAsync(alert.LocationName);
        if (alertDb is not null)
        {
            alertDb.Active = alert.Active;
            alertDb.UpdateTime = alert.UpdateTime;
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            await _dbContext.Alerts.AddAsync(alert);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteAlertAsync(string location)
    {
        if (!await _dbContext.Alerts!.AnyAsync(a => a.LocationName == location))
        {
            _dbContext.Remove(new Alert() { LocationName = location });
            await _dbContext.SaveChangesAsync();
        }
    }

    public Task DeleteAlertByLocation(string location)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsAlertExits(string location)
    {
        return await _dbContext.Alerts!.AnyAsync(a => a.LocationName == location);
    }
}