using AlertsApi.Domain.Entities;

namespace AlertsApi.Api.Services;

public interface IMessagesService
{
    Task<IEnumerable<DbMessage>> GetAllAsync();
    Task<DbMessage?> GetByIdAsync(int id);
}