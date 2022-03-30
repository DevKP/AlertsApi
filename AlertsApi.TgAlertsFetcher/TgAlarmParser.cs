using System.Text.RegularExpressions;
using AlertsApi.TgAlertsFramework.Exceptions;
using AlertsApi.TgAlertsFramework.Models;
using TL;
using WTelegram;

namespace AlertsApi.TgAlertsFramework;

public class TgAlarmParser
{
    private readonly Client _client;
    private readonly InputPeerChannel _channel;
    private readonly Regex _regionRegex = new("(?<location>[\\s\\w]+([\\s\\w\\.-]+)?)\\n", RegexOptions.Compiled | RegexOptions.Singleline);

    public Func<IEnumerable<TgAlert>, Task>? OnUpdates;
    public static Action<int, string> Log
    {
        get => Helpers.Log;
        set => Helpers.Log = value;
    }

    public TgAlarmParser(string channelName, string phoneNumber, string? sessionPath = null)
    {
        var sessionStore = OpenFileStream(sessionPath);

        var configs = new ClientConfig
        {
            PhoneNumber = phoneNumber
        };

        _client = new Client(configs.ConfigsProvider, sessionStore);
        _client.LoginUserIfNeeded();

        _client.Update += ClientOnUpdate;

        var resolved = _client.Contacts_ResolveUsername(channelName)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        _channel = resolved.Chat.ToInputPeer() as InputPeerChannel
            ?? throw new InvalidOperationException();
    }

    private static FileStream? OpenFileStream(string? sessionPath)
    {
        if (sessionPath is null)
            return null;

        return File.Open(sessionPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
    }

    public async Task<IEnumerable<TgAlert>> GetHistoryAsync(TimeSpan period)
    {
        var until = DateTime.Now - period;
        var offset = 0;

        var alerts = new List<TgAlert>();

        do
        {
            var messagesChunk = await _client.Messages_GetHistory(_channel, add_offset: offset, limit: 10);
            if (!messagesChunk.Messages.Any()) 
                break;

            var messages = messagesChunk.Messages
                .Where(msg => msg is Message)
                .Cast<Message>()
                .ToList();

            if(messages[^1].Date.AddHours(3) < until)
                break;

            offset += 10;

            alerts.AddRange(ParseMessages(messages));

            await Task.Delay(100);
        } while (true);

        return alerts;
    }

    private TgAlert ParseMessage(Message message)
    {
        var match = _regionRegex.Match(message.message);
        var location = match.Groups["location"].Value.Trim();

        if (string.IsNullOrWhiteSpace(location))
            throw new UnableToParseMessageException();

        var alarmState = !message.message.Contains("відбій", StringComparison.OrdinalIgnoreCase);

        var alarm = new TgAlert()
        {
            LocationTitle = location,
            Active = alarmState,
            FetchedAt = message.Date + TimeSpan.FromHours(3)
        };

        return alarm;
    }

    private IEnumerable<TgAlert> ParseMessages(IEnumerable<Message> messages)
    {
        foreach (var message in messages)
        {
            TgAlert tgAlert;
            try
            {
                tgAlert = ParseMessage(message);
            }
            catch (UnableToParseMessageException)
            {
                continue;
            }

            yield return tgAlert;
        }
    }

    private void ClientOnUpdate(IObject obj)
    {
        if (obj is not UpdatesBase updates)
            return;

        var messages = updates.UpdateList
            .Where(update => update is UpdateNewMessage msg &&
                             msg.message.Peer.ID == _channel.channel_id)
            .Cast<UpdateNewMessage>()
            .Select(update => update.message)
            .Cast<Message>();
        
        OnUpdates?.Invoke(ParseMessages(messages))
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }
}