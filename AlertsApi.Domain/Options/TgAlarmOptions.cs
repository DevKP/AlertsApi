namespace AlertsApi.Domain.Options;

public class TgAlarmOptions
{
    public const string ConfigKey = "TgAlarm";

    public string? PhoneNumber { get; init; }
    public string? SessionStorePath { get; init; }
    public string? ChannelName { get; set; }
}