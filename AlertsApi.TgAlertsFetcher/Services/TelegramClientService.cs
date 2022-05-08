using AlertsApi.WTelegram.Hosting.Exceptions;
using AlertsApi.WTelegram.Hosting.Options;
using Microsoft.Extensions.Logging;
using TL;
using WTelegram;

namespace AlertsApi.WTelegram.Hosting.Services;

public class TelegramClientService : ITelegramClientService
{
    private readonly Client _client;

    private Func<IEnumerable<Message>, Task>? _updatesListener;

    public TelegramClientService(string sessionStorePath,
        CustomConfigProvider configProvider)
    {
        _client = CreateClient(sessionStorePath, configProvider);

        _client.Update += OnUpdate;
    }

    private static Client CreateClient(string sessionPath, CustomConfigProvider configProvider)
    {
        var sessionStore = OpenFileStream(sessionPath);
        return new Client(configProvider.ConfigsProvider, sessionStore);
    }

    public async Task LoginUserIfNeeded()
    {
        await _client.LoginUserIfNeeded().ConfigureAwait(false);
    }

    public void AddMessagesListener(Func<IEnumerable<Message>, Task> listener)
    {
        _updatesListener += listener;
    }

    public async Task<InputPeerChannel> GetChannelPeerAsync(string username)
    {
        var peer = await _client.Contacts_ResolveUsername(username)
            ?? throw new ChatNotFoundException();

        var channelPeer = peer.Chat.ToInputPeer() as InputPeerChannel
            ?? throw new ChatNotFoundException();

        return channelPeer;
    }

    public async Task<IEnumerable<Message>> GetHistoryFromIdAsync(InputPeerChannel channel, int messageId)
    {
        const int maxMessageOffset = 3000;
        const int messageLimit = 100;

        var messages = new List<Message>();
        for (var offset = 0; offset < maxMessageOffset; offset += messageLimit)
        {
            var messagesChunk = await _client.Messages_GetHistory(channel, min_id: messageId, limit: messageLimit, add_offset: offset)
                .ConfigureAwait(false);

            var messagesChunkList = messagesChunk.Messages
                .Where(msg => msg is Message)
                .Cast<Message>().ToList();

            messages.AddRange(messagesChunkList);

            if(messagesChunkList.Count < messageLimit)
                break;
        }

        return messages;
    }

    public async Task<IEnumerable<Message>> GetHistoryFromDateAsync(InputPeerChannel channel, DateTime dateFrom)
    {
        var oldestMessages = await _client.Messages_GetHistory(channel, offset_date: dateFrom, limit: 1);
        return await GetHistoryFromIdAsync(channel, oldestMessages.Messages.First().ID);
    }

    private static FileStream OpenFileStream(string sessionPath)
    {
        return File.Open(sessionPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
    }

    private void OnUpdate(IObject obj)
    {
        if (_updatesListener is null)
            return;

        if (obj is not UpdatesBase updates)
            return;

        var messages = GetOnlyNewMessages(updates);

        _updatesListener.Invoke(messages)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    private static IEnumerable<Message> GetOnlyNewMessages(UpdatesBase updates)
    {
        var messages = updates.UpdateList
            .Where(update => update is UpdateNewMessage)
            .Cast<UpdateNewMessage>()
            .Select(update => update.message)
            .Where(message => message is Message)
            .Cast<Message>();

        return messages;
    }
}