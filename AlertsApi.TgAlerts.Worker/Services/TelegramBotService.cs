using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace AlertsApi.TgAlerts.Worker.Services
{
    public class TelegramBotService : ITelegramBotService
    {
        public Task SendMessageAsync(string message)
        {
            throw new NotImplementedException();
        }

        public async Task Test()
        {
            var botClient = new TelegramBotClient("5336718267:AAFQNm6oHuZMMWX1i6udAh-5kX-vA1vUbYI");

            var me = await botClient.GetMeAsync();
            botClient.StartReceiving(async (client, update, _) =>
            {
                if (update.Type == UpdateType.Message)
                {
                    var message = update.Message;
                    if (message.Type == MessageType.Text)
                    {
                        if (message.Text == "hello")
                        {
                            var reply = "Hello! I'm a bot";
                            await botClient.SendTextMessageAsync(message.Chat.Id, reply);
                        }
                    }
                }
            }, (client, exc, token) => {  });

        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
        }
    }
}
