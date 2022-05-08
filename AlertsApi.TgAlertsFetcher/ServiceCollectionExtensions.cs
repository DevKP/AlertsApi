using AlertsApi.WTelegram.Hosting.Options;
using AlertsApi.WTelegram.Hosting.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WTelegram;

namespace AlertsApi.WTelegram.Hosting;

public static class ServiceCollectionExtensions
{
    public static void AddTelegramClient(this IServiceCollection services, Action<ClientOptionsBuilder> optionsAction)
    {
        services.AddSingleton<ITelegramClientService, TelegramClientService>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<TelegramClientService>>();
            Helpers.Log = (i, msg) => logger?.Log((LogLevel) i,"[WTelegram]:{Message}", msg);

            var options = new ClientOptionsBuilder();
            optionsAction.Invoke(options);

            var client = new TelegramClientService(options.GetSessionsStorePath(), options.GetConfigProvider());
            return client;
        });
    }
}