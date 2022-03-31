using AlertsApi.TgAlerts.Worker.Models;
using TL;

namespace AlertsApi.TgAlerts.Worker.Services;

public interface IMessagesParserService
{
    IEnumerable<TgAlert> ParseMessages(IEnumerable<Message> messages);
}