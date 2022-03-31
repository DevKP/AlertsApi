namespace AlertsApi.TgAlerts.Worker.Config;

public class ClientConfig
{
    private static readonly Dictionary<string, string?> ClientConfigs;

    public string? PhoneNumber { get; init; }

    static ClientConfig()
    {
        ClientConfigs = new Dictionary<string, string?>
        {
            {"api_id", "19657090"},
            {"api_hash", "01d7dcd1490c1b6f89985882379ad0ab"},
            {"session_pathname", null},
            {"session_key", null},
            {"server_address", "149.154.167.50:443"},
            {"device_model", "model"},
            {"system_version", "win10"},
            {"app_version", "0.1"},
            {"system_lang_code", "en"}
        };
    }

    public string? ConfigsProvider(string key)
    {
        return key switch
        {
            "phone_number" => PhoneNumber,
            null => null,
            _ => StaticConfigsProvider(key)
        };
    }

    public static string? StaticConfigsProvider(string key)
    {
        if (ClientConfigs.TryGetValue(key, out var value))
            return value;

        return null;
    }
}