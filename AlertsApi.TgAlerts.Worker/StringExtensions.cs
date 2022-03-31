namespace AlertsApi.TgAlerts.Worker;

public static class StringExtensions
{
    public static bool IsEmptyOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);
}