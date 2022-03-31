using System.Text;
using AlertsApi.Domain.Entities;
using AlertsApi.Domain.Options;
using AlertsApi.Domain.Repositories;
using AlertsApi.WTelegram.Hosting.Options;
using AlertsApi.WTelegram.Hosting.Services;
using AutoMapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TL;

namespace AlertsApi.TgAlerts.Worker.Services;

public class TelegramFetcherService : BackgroundService
{ 
    private readonly IAlertsService _alertsService;
    private readonly IMessageRepository _messageRepository;
    private readonly ITelegramClientService _client;
    private readonly IMessagesParserService _messagesParser;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    private readonly ClientOptions _alarmOptions;
    private InputPeerChannel? _channel;
    private readonly DateTime _dateFrom;

    public TelegramFetcherService(IAlertsService alertsService, IOptions<ClientOptions> options,
        ILogger<TelegramFetcherService> logger, IMessageRepository messageRepository, IMapper mapper,
        IMessagesParserService messagesParser, ITelegramClientService client)
    {
        ArgumentNullException.ThrowIfNull(alertsService, nameof(alertsService));
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        _client = client;
        _messagesParser = messagesParser;
        _alertsService = alertsService;
        _messageRepository = messageRepository;
        _mapper = mapper;
        _logger = logger;
        _alarmOptions = options.Value;
        _dateFrom = DateTime.UtcNow.AddHours(-(_alarmOptions.InitialMessagesHours ?? 24));

        Console.OutputEncoding = Encoding.UTF8;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _client.LoginUserIfNeeded();

            _channel = await _client.GetChannelPeerAsync(_alarmOptions.ChannelName!).ConfigureAwait(false);
            _logger.LogInformation("Start to fetching history from tg channel {ChannelId}...", _channel.channel_id);

            var newMessages = (await GetNewMessages(_channel)).ToList();
            _logger.LogInformation("Loaded {Number} new messages.", newMessages.Count);

            var alerts = _messagesParser.ParseMessages(newMessages);
            await _alertsService.UpdateAlertsAsync(alerts);

            var newMessagesEntities = _mapper.Map<IEnumerable<Message>, IEnumerable<DbMessage>>(newMessages);
            await _messageRepository.InsertRangeAsync(newMessagesEntities);

            _logger.LogInformation("Start monitoring real time updates.");
            _client.AddMessagesListener(NewMessagesListenerAsync);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Fatal error");
        }

        Console.ReadLine();
    }

    private async Task NewMessagesListenerAsync(IEnumerable<Message> messages)
    {
        var filteredMessages = messages.Where(m => m.Peer.ID == _channel!.channel_id).ToList();
        foreach (var message in filteredMessages)
            _logger.LogInformation("New message from channel: {Message}", message.message);

        var dbMessages = _mapper.Map<IEnumerable<Message>, IEnumerable<DbMessage>>(filteredMessages);
        await _messageRepository.InsertRangeAsync(dbMessages);

        var alerts = _messagesParser.ParseMessages(filteredMessages);
        await _alertsService.UpdateAlertsAsync(alerts);
    }

    private async Task<IEnumerable<Message>> GetNewMessages(InputPeerChannel channel)
    {
        var lastMessage = await _messageRepository.GetNewestAsync().ConfigureAwait(false);
        if (lastMessage is null || lastMessage.Date < _dateFrom)
        {
            _logger.LogInformation("Message table is empty or last message is to old! Loading history from date: {Date}", _dateFrom);

            return await _client.GetHistoryFromDateAsync(channel, _dateFrom).ConfigureAwait(false);
        }

        _logger.LogInformation("Loading message history from ID: {MessageId}", lastMessage.Id);

        var messages = (await _client.GetHistoryFromIdAsync(channel, lastMessage.Id));
        return messages;
    }
}