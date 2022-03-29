using System.Text;
using AlertsApi.TgAlertsFramework;
using AlertsApi.TgAlertsFramework.Models;

Console.OutputEncoding = Encoding.UTF8;

var parser = new TgAlarmParser("testalertstest");
parser.OnUpdates += alertsUpdates =>
{
    foreach (var alert in alertsUpdates)
    {
        ShowAlert(alert);
    }
};

var alerts = parser.GetHistoryAsync(TimeSpan.FromHours(5));

await foreach (var alert in alerts)
{
    ShowAlert(alert);
}

Console.Read();

//-----

void ShowAlert(TgAlert alert)
{
    Console.WriteLine($"{alert.LocationTitle}, {alert.Active}, {alert.FetchedAt.ToShortTimeString()}");
}