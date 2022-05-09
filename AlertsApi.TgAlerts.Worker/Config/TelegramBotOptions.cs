namespace AlertsApi.TgAlerts.Worker.Config;

public class TelegramBotOptions
{
    public const string ConfigKey = "TelegramBot";

    public string Token { get; set; }
}