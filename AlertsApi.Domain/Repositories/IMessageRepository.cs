using AlertsApi.Domain.Entities;

namespace AlertsApi.Domain.Repositories;

public interface IMessageRepository
{
    Task InsertAsync(DbMessage message);
    Task InsertRangeAsync(IEnumerable<DbMessage> message);
    Task<IEnumerable<DbMessage>> GetAllAsync();
    Task<DbMessage?> GetById(int id);
    Task<DbMessage?> GetNewestAsync();
    //Task UpdateAlertAsync(Alert alert);
    //Task DeleteAlertAsync(string location);
    //Task DeleteAlertByLocation(string location);
    //Task<bool> IsAlertExits(string location);
}