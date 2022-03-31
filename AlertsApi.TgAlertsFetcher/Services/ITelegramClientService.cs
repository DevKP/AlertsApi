using TL;

namespace AlertsApi.WTelegram.Hosting.Services;

public interface ITelegramClientService
{
    public Task LoginUserIfNeeded();
    void AddMessagesListener(Func<IEnumerable<Message>, Task> listener);
    Task<InputPeerChannel> GetChannelPeerAsync(string username);
    Task<IEnumerable<Message>> GetHistoryFromIdAsync(InputPeerChannel channel, int messageId);
    Task<IEnumerable<Message>> GetHistoryFromDateAsync(InputPeerChannel channel, DateTime dateFrom);
}