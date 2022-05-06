using System.Text.RegularExpressions;
using AlertsApi.TgAlerts.Worker.Models;
using TL;

namespace AlertsApi.TgAlerts.Worker.Services;

public class MessagesParserService : IMessagesParserService
{
    private readonly string[] _alertOffKeywords = {"🟢", "🟡"};
    private readonly Regex _locationHashTagRegex = new("#(?<location>[\\w_]+)", RegexOptions.Compiled | RegexOptions.Singleline);
    private readonly Regex _locationNameRegex = new("в (?<location>[\\w \\-\\.\\']+)$", RegexOptions.Multiline | RegexOptions.Compiled);

    public IEnumerable<TgAlert> ParseMessages(IEnumerable<Message> messages)
    {
        foreach (var message in messages)
        {
            var locationHashTag = GetLocation(message.message);
            if (locationHashTag.IsEmptyOrWhiteSpace())
                continue;
            
            var locationName = GetLocationName(message.message);
            var alertState = IsAlertOn(message.message);
            var alert = new TgAlert
            {
                LocationTitle = locationName,
                LocationHashTag = locationHashTag,
                FetchedAt = message.Date,
                Active = alertState,
                OriginalMessage = message.message
            };

            yield return alert;
        }
    }

    private bool IsAlertOn(string message)
    {
        return !_alertOffKeywords.Any(keyword => message.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    private string GetLocation(string message)
    {
        var match = _locationHashTagRegex.Match(message);
        return match.Success ? match.Groups["location"].Value : string.Empty;
    }

    private string GetLocationName(string message)
    {
        var match = _locationNameRegex.Match(message);
        return match.Success ? match.Groups["location"].Value.TrimEnd('.') : string.Empty;
    }
}