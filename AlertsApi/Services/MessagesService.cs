using AlertsApi.Domain.Entities;
using AlertsApi.Domain.Repositories;

namespace AlertsApi.Api.Services;

public class MessagesService : IMessagesService
{
    private readonly IMessageRepository _messageRepository;

    public MessagesService(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<IEnumerable<DbMessage>> GetAllAsync()
    {
        return await _messageRepository.GetAllAsync();
    }

    public async Task<DbMessage?> GetByIdAsync(int id)
    {
        return await _messageRepository.GetById(id);
    }
}