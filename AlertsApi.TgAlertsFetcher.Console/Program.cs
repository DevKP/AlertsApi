using System.Text;
using AlertsApi.TgAlertsFramework;
using AlertsApi.TgAlertsFramework.Models;

Console.OutputEncoding = Encoding.UTF8;

var parser = new TgAlarmParser("testalertstest", "+380995031137");
parser.OnUpdates += async alertsUpdates =>
{
    foreach (var alert in alertsUpdates)
    {
        ShowAlert(alert);
    }

    await Task.CompletedTask;
};

var alerts = await parser.GetHistoryAsync(TimeSpan.FromHours(5));

foreach (var alert in alerts)
{
    ShowAlert(alert);
}

Console.Read();

//-----

void ShowAlert(TgAlert alert)
{
    Console.WriteLine($"{alert.LocationTitle}, {alert.Active}, {alert.FetchedAt.ToShortTimeString()}");
}