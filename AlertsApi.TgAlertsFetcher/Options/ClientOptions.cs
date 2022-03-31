namespace AlertsApi.WTelegram.Hosting.Options;

public class ClientOptions
{
    public const string ConfigKey = "TgAlarm";

    public string? PhoneNumber { get; init; }
    public string? SessionStorePath { get; init; }
    public string? ChannelName { get; set; }

    public int? InitialMessagesHours;
}