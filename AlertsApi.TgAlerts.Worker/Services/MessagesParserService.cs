using System.Text.RegularExpressions;
using AlertsApi.TgAlerts.Worker.Models;
using TL;

namespace AlertsApi.TgAlerts.Worker.Services;

class MessagesParserService : IMessagesParserService
{
    private readonly string[] _alertOffKeywords = {"🟢", "🟡"};
    private readonly Regex _locationRegex = new("#(?<location>[\\w_]+)", RegexOptions.Compiled | RegexOptions.Singleline);
  
    public IEnumerable<TgAlert> ParseMessages(IEnumerable<Message> messages)
    {
        foreach (var message in messages)
        {
            var locationMatch = _locationRegex.Match(message.message);
            if (!locationMatch.Success)
                continue;

            var locationHashTag = locationMatch.Groups["location"].Value;
            if(locationHashTag.IsEmptyOrWhiteSpace())
                continue;
            
            var location = FormatLocationTag(locationHashTag);
            var alertState = GetAlertState(message.message);
            var alert = new TgAlert
            {
                LocationTitle = location,
                FetchedAt = message.Date,
                Active = alertState
            };

            yield return alert;
        }
    }

    private bool GetAlertState(string message)
    {
        var alarmNotActive =
            _alertOffKeywords.Any(keyword => message.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        return !alarmNotActive;
    }

    private static string FormatLocationTag(string locationTag)
    {
        return locationTag.Trim().Replace('_', ' ');
    }
}