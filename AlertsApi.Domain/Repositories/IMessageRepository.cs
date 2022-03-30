using AlertsApi.Domain.Entities;

namespace AlertsApi.Domain.Repositories;

public interface IMessageRepository
{
    Task InsertAsync(MessageEntity message);
    Task InsertRangeAsync(IEnumerable<MessageEntity> message);
    Task<IEnumerable<MessageEntity>> GetAllAsync();
    Task<MessageEntity?> GetById(int id);
    Task<MessageEntity?> GetLatestAsync(string location);
    //Task UpdateAlertAsync(Alert alert);
    //Task DeleteAlertAsync(string location);
    //Task DeleteAlertByLocation(string location);
    //Task<bool> IsAlertExits(string location);
}