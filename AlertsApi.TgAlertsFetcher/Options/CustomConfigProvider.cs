namespace AlertsApi.WTelegram.Hosting.Options;

public class CustomConfigProvider
{
    private readonly Dictionary<string, string?> _configDictionary;

    public CustomConfigProvider(Dictionary<string, string?> configDictionary)
    {
        _configDictionary = configDictionary;
    }

    public string? GetValue(string name)
    {
        if (!_configDictionary.TryGetValue(name, out var value))
            return null;

        return value;
    }

    public string? ConfigsProvider(string key)
    {
        return key switch
        {
            null => null,
            _ => GetValue(key)
        };
    }
}