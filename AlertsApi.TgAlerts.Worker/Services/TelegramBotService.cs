using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AlertsApi.Domain.Entities;
using AlertsApi.Domain.Repositories;
using AlertsApi.TgAlerts.Worker.Models;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace AlertsApi.TgAlerts.Worker.Services
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly TelegramBotClient _client;
        private readonly Dictionary<long, string> _alertSubsriptions;
        private readonly List<Alert> _alerts;
        private readonly ILogger _logger;

        public TelegramBotService(IAlertRepository _alertsRepository, ILogger<TelegramBotService> logger)
        {
            _client = new TelegramBotClient("5336718267:AAFQNm6oHuZMMWX1i6udAh-5kX-vA1vUbYI");
            _alertSubsriptions = new Dictionary<long, string>();
            _alerts = _alertsRepository.GetAllAlertsAsync().Result.ToList();
            _logger = logger;
        }

        public async Task Invoke(string locationName, string message)
        {
            var users = _alertSubsriptions.Where(s => s.Value.Contains(locationName.Replace("м ", "")));
            foreach(var user in users)
            {
                _logger.LogInformation($"Sending notification about {locationName} to {user.Key}");
                await _client.SendTextMessageAsync(user.Key, message);
            }
        }

        public async Task Start()
        {
            var me = await _client.GetMeAsync();
            _client.StartReceiving(async (client, update, _) =>
            {
                if (update.Type == UpdateType.Message)
                {
                    var message = update.Message;
                    if (message.Type == MessageType.Text)
                    {
                        if(message.Text.Equals("відписатись", StringComparison.OrdinalIgnoreCase))
                        {
                            _alertSubsriptions.Remove(message.Chat.Id);
                            await _client.SendTextMessageAsync(message.Chat.Id, "Ти не будеш отримувати сповіщення.");
                            return;
                        }

                        var alert = _alerts.FirstOrDefault(a => a.LocationName.Contains(message.Text, StringComparison.OrdinalIgnoreCase));
                        if(alert is null)
                        {
                            await _client.SendTextMessageAsync(message.Chat.Id, "Не знайдено.");
                            return;
                        }

                        if(!_alertSubsriptions.TryAdd(message.Chat.Id, alert.LocationName))
                        {
                            await _client.SendTextMessageAsync(message.Chat.Id, "Ти вже отримуєш сповіщення по цій області.");
                            return;
                        }

                        _logger.LogInformation($"User {message.Chat.Id} subscribed to {alert.LocationName}");
                        await _client.SendTextMessageAsync(message.Chat.Id, $"Коли буде тривога у {alert.LocationName}, я обов'язково повідомлю!");
                    }
                }
            }, (client, exc, token) => {  });

        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
        }
    }
}
