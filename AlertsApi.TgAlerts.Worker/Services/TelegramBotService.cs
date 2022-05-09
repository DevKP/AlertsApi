using Microsoft.Extensions.Logging;
using AlertsApi.Domain.Entities;
using AlertsApi.Domain.Repositories;
using AlertsApi.TgAlerts.Worker.Config;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AlertsApi.TgAlerts.Worker.Services
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly TelegramBotClient _client;
        private readonly ISubscriptionsService _subscriptionsService;
        private readonly IAlertRepository _alertRepository;
        private readonly ILogger _logger;

        public TelegramBotService(ILogger<TelegramBotService> logger, IAlertRepository alertRepository,
            ISubscriptionsService subscriptionsService, IOptions<TelegramBotOptions> options)
        {
            var botOptions = options.Value ?? throw new ArgumentNullException(nameof(options));

            _client = new TelegramBotClient(botOptions.Token);
            _logger = logger;
            _alertRepository = alertRepository;
            _subscriptionsService = subscriptionsService;
        }

        public async Task Notify(string locationHashTag, Alert alert)
        {
            var subscriptions = await _subscriptionsService.GetSubscriptionsByHashTagAsync(locationHashTag);
            foreach (var subscription in subscriptions)
            {
                _logger.LogInformation($"Sending notification about {locationHashTag} to {subscription.UserId}");
                
                if (alert.Active)
                {
                    await _client.SendStickerAsync(subscription.UserId, "CAACAgIAAxkBAAEEoCRicpAJlItgHnWjMyF9tgyZs8SaFQACGRQAAj-hgUlhPKx_Qd2bhiQE");
                    await _client.SendTextMessageAsync(subscription.UserId, $"🔴 Тривога у {alert.LocationName}! Усі в укриття!");
                }
                else
                {
                    await _client.SendStickerAsync(subscription.UserId, "CAACAgIAAxkBAAEEoCZicpAMfHc_0DgwP2Jjcg3AWBPNjgACLRoAAruOgUmgxH0WL_Q7xCQE");
                    await _client.SendTextMessageAsync(subscription.UserId, $"🟢 Відбій тривоги у {alert.LocationName}! Насолоджуйтесь життям :3");
                }
            }
        }

        public void Start()
        {
            _client.StartReceiving(async (client, update, cancellationToken) =>
            {
                if (update.Type != UpdateType.Message)
                {
                    return;
                }

                var message = update.Message;
                if (message is null || message.Type != MessageType.Text)
                {
                    return;
                }

                if (message.Text!.Equals("/start", StringComparison.OrdinalIgnoreCase))
                {
                    await _client.SendTextMessageAsync(message.Chat.Id, "Привіт друже! Я можу повідомляти тебе кожного разу, коли ставатиме небезпечно бути поза укриттям :з", cancellationToken: cancellationToken);
                    await _client.SendTextMessageAsync(message.Chat.Id, "Будь-ласочка не нехтуй правилами безпеки, бо я буду сумувати 😿", cancellationToken: cancellationToken);
                    await _client.SendTextMessageAsync(message.Chat.Id, "Надішли мені назву міста чи області де ти хочеш слідкувати за повітряною тривогою. Ммурь ^-^", cancellationToken: cancellationToken);
                    return;
                }

                if (message.Text!.Equals("/subscriptions", StringComparison.OrdinalIgnoreCase))
                {
                    var subscriptions = (await _subscriptionsService.GetUserSubscriptionsAsync(message.Chat.Id)).ToList();
                    if (!subscriptions.Any())
                    {
                        await _client.SendTextMessageAsync(message.Chat.Id, "Ти не отримуєш сповіщення.", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
                        return;
                    }

                    var list = subscriptions.Select(sub => sub.Alert?.LocationName).ToArray();
                    await _client.SendTextMessageAsync(message.Chat.Id, string.Join('\n', list), cancellationToken: cancellationToken);
                    return;
                }

                if (message.Text!.Equals("/alerts", StringComparison.OrdinalIgnoreCase))
                {
                    var activeAlerts = (await _alertRepository.GetOnlyActiveAsync()).ToList();
                    if (!activeAlerts.Any())
                    {
                        await _client.SendTextMessageAsync(message.Chat.Id, "Немає активної тривоги. ❤️", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
                        return;
                    }

                    var list = activeAlerts.Select(a => $"🔴 {a.StartTime:HH:mm} {a.LocationName}").ToArray();
                    await _client.SendTextMessageAsync(message.Chat.Id, $"Наразі тривога триває:\n\n{string.Join("\n", list)}", cancellationToken: cancellationToken);
                    return;
                }

                if (message.Text!.Equals("/unsubscribeall", StringComparison.OrdinalIgnoreCase))
                {
                    await _subscriptionsService.RemoveAllUserSubscriptionsAsync(message.Chat.Id);
                    await _client.SendTextMessageAsync(message.Chat.Id, "Ти не будеш отримувати сповіщення.", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
                    return;
                }

                var alerts = (await _alertRepository.GetAllAlertsAsync()).ToList();
                var locations = alerts.Where(a =>
                    a.LocationName!.Contains(message.Text, StringComparison.OrdinalIgnoreCase)).ToList();

                if (locations.Count > 1)
                {
                    var buttons = locations.Select(l => new[] { new KeyboardButton(l.LocationName!) });
                    var keyboard = new ReplyKeyboardMarkup(buttons);
                    await _client.SendTextMessageAsync(message.Chat.Id, "Вибери зі списку", replyMarkup: keyboard, cancellationToken: cancellationToken);
                    return;
                }

                if (message.Text!.Equals("відписатись", StringComparison.OrdinalIgnoreCase))
                {
                    await _subscriptionsService.RemoveAllUserSubscriptionsAsync(message.Chat.Id);
                    await _client.SendTextMessageAsync(message.Chat.Id, "Ти не будеш отримувати сповіщення.", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
                    return;
                }

                var alert = alerts.FirstOrDefault(a => a.LocationName!.Contains(message.Text, StringComparison.OrdinalIgnoreCase));
                if (alert is null)
                {
                    await _client.SendTextMessageAsync(message.Chat.Id, "Я не зміг знайти нічого у своїй базі.", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
                    return;
                }

                if (await _subscriptionsService.IsSubscriptionExistAsync(message.Chat.Id, alert.LocationHashTag))
                {
                    await _client.SendTextMessageAsync(message.Chat.Id, "Ти вже отримуєш сповіщення по цій області.", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
                    return;
                }

                await _subscriptionsService.SubscribeUserAsync(message.Chat.Id, alert.LocationHashTag);

                _logger.LogInformation($"User {message.Chat.Id} subscribed to {alert.LocationHashTag}");
                await _client.SendTextMessageAsync(message.Chat.Id, $"Коли буде тривога у {alert.LocationName}, я обов'язково повідомлю!", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
            }, (client, exc, token) => {  });

        }
    }
}
