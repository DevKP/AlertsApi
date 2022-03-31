namespace AlertsApi.WTelegram.Hosting.Options;

public class ClientOptionsBuilder
{
    private readonly Dictionary<string, string?> _configDictionary;
    private string? _sessionStorePath;

    public ClientOptionsBuilder()
    {
        _configDictionary = new Dictionary<string, string?>();
    }

    public ClientOptionsBuilder SetCustomConfig(string name, string value)
    {
        _configDictionary.TryAdd(name, value);
        return this;
    }

    public ClientOptionsBuilder SetSessionsStore(string path)
    {
        _sessionStorePath = path;
        return this;
    }

    public string GetSessionsStorePath()
    {
        return _sessionStorePath!;
    }

    public CustomConfigProvider GetConfigProvider()
    {
        return new CustomConfigProvider(_configDictionary);
    }
}