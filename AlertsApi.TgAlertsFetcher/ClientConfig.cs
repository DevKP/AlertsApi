namespace AlertsApi.TgAlertsFramework;

public class ClientConfig
{
    private static readonly Dictionary<string, string?> ClientConfigs;

    static ClientConfig()
    {
        ClientConfigs = new Dictionary<string, string?>
        {
            {"api_id", "19657090"},
            {"api_hash", "01d7dcd1490c1b6f89985882379ad0ab"},
            {"phone_number", "+380995031137"},
            {"session_pathname", null},
            {"session_key", null},
            {"server_address", "149.154.167.50:443"},
            {"device_model", "model"},
            {"system_version", "win10"},
            {"app_version", "0.1"},
            {"system_lang_code", "en"}
        };
    }

    public static string? ConfigsProvider(string key)
    {
        if (ClientConfigs.TryGetValue(key, out var value))
            return value;

        return null;
    }
}