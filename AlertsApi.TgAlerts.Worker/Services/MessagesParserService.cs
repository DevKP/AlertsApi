﻿using System.Text.RegularExpressions;
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
            var locationHashTag = GetLocation(message.message);
            if (locationHashTag.IsEmptyOrWhiteSpace())
                continue;
            
            var location = FormatLocationTag(locationHashTag);
            var alertState = IsAlertOn(message.message);
            var alert = new TgAlert
            {
                LocationTitle = location,
                LocationHashTag = locationHashTag,
                FetchedAt = message.Date,
                Active = alertState,
                OriginalMessage = message.message
            };

            yield return alert;
        }
    }

    public bool IsAlertOn(string message)
    {
        return !_alertOffKeywords.Any(keyword => message.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    public string GetLocation(string message)
    {
        var match = _locationRegex.Match(message);
        return match.Success ? match.Groups["location"].Value : string.Empty;
    }

    private static string FormatLocationTag(string locationTag)
    {
        var formattedLocationTag = locationTag.Trim().Replace('_', ' ');
        if (formattedLocationTag.StartsWith("м "))
        {
            return formattedLocationTag.Replace("м ", "м. ");
        }

        return formattedLocationTag;
    }
}