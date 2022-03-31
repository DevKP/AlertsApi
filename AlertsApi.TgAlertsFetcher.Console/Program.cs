using System.Text;
using TeleSharp.TL;
using TeleSharp.TL.Channels;
using TeleSharp.TL.Contacts;
using TeleSharp.TL.Messages;
using TLSharp.Core;

Console.OutputEncoding = Encoding.UTF8;


var client = new TelegramClient(19657090, "01d7dcd1490c1b6f89985882379ad0ab");
await client.ConnectAsync();

if (!client.IsUserAuthorized())
{
    var hash = await client.SendCodeRequestAsync("+380995031137");
    var code = Console.ReadLine();
    var user = await client.MakeAuthAsync("+380995031137", hash, code);
    //var messages = await client.GetHistoryAsync(new TLInputPeerChannel(){AccessHash = channel.}, minId: 5);
}

var peer = await client.SendRequestAsync<TLResolvedPeer>(new TLRequestResolveUsername() { Username = "testalertstest" });
var channel = peer.Chats.Cast<TLChannel>().First(c => c.Id == (peer.Peer as TLPeerChannel).ChannelId);
var messages =
    await client.GetHistoryAsync(new TLInputPeerChannel() {AccessHash = channel.AccessHash.Value, ChannelId = channel.Id},
        minId: 50);

Console.WriteLine((messages as TLMessages).Messages);



//var parser = new TgAlarmParser("testalertstest", "+380995031137", "test");
//parser.OnUpdates += async alertsUpdates =>
//{
//    foreach (var alert in alertsUpdates)
//    {
//        ShowAlert(alert);
//    }

//    await Task.CompletedTask;
//};

//var alerts = await parser.GetHistoryAsync(TimeSpan.FromHours(5));

//foreach (var alert in alerts)
//{
//    ShowAlert(alert);
//}

//Console.Read();

////-----

//void ShowAlert(TgAlert alert)
//{
//    Console.WriteLine($"{alert.LocationTitle}, {alert.Active}, {alert.FetchedAt.ToShortTimeString()}");
//}