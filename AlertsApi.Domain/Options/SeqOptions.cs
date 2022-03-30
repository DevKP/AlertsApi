namespace AlertsApi.Domain.Options;

public class SeqOptions
{
    public const string ConfigKey = "Seq";

    public string? ServerUrl { get; init; }

    public string? ApiKey { get; init; }
}