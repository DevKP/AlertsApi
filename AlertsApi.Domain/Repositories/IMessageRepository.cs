using AlertsApi.Domain.Entities;

namespace AlertsApi.Domain.Repositories;

public interface IMessageRepository
{
    Task AddAsync(DbMessage message);
    Task AddOrUpdateAsync(DbMessage message);
    Task AddOrUpdateRangeAsync(IEnumerable<DbMessage> message);
    Task<IEnumerable<DbMessage>> GetAllAsync();
    Task<DbMessage?> GetById(int id);
    Task<DbMessage?> GetNewestAsync();
    //Task UpdateAlertAsync(Alert alert);
    //Task DeleteAlertAsync(string location);
    //Task DeleteAlertByLocation(string location);
    //Task<bool> IsAlertExits(string location);
}