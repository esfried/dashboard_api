using Microsoft.AspNetCore.SignalR;
using NuGet.Protocol.Plugins;
using System.Collections.Concurrent;
using System.Diagnostics;

public class ClientInfo
{
    public HubRequestCommand Request { get; set; }
    public DateTime HeartBeat { get; set; }
}

public class DashboardHub : Hub<IDashboardHub>
{
    public static readonly ConcurrentDictionary<string, ClientInfo> _clientInfo = new();
    private System.Timers.Timer _heartbeatTimer;

    public DashboardHub()
    {
        _heartbeatTimer = new System.Timers.Timer(TimeSpan.FromSeconds(1).TotalMilliseconds);
        _heartbeatTimer.Elapsed += _heartbeatTimer_Elapsed;
        _heartbeatTimer.Start();
    }

    // CheckHeartBeats
    private void _heartbeatTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        var now = DateTime.UtcNow;
        _heartbeatTimer.Stop();

        try
        {
            ConcurrentDictionary<string, ClientInfo> tmpDict = new();
            foreach (var pair in _clientInfo)
                tmpDict.TryAdd(pair.Key, pair.Value);

            foreach (var item in tmpDict)
            {
                var connectionId = item.Key;

                if ((now - _clientInfo[connectionId].HeartBeat).TotalSeconds > 10)
                {
                    Trace.WriteLine($"************************* REMOVING DEAD Client: {connectionId} !");
                    Groups.RemoveFromGroupAsync(connectionId, _clientInfo[connectionId].Request.ToString());
                    _clientInfo.TryRemove(connectionId, out _);
                }
            }
        }
        catch (Exception ex)
        {

        }

        _heartbeatTimer.Start();
    }

    public async Task SendDataToScreenOne(DtoScreenOneData data) =>
        await Clients.All.Response_For_Screen_One_Data(data);

    public async Task SendDataToScreenTwo(DtoScreenTwoData data) =>
        await Clients.All.Response_For_Screen_Two_Data(data);

    public async Task SendDataToScreenThree(DtoScreenThreeData data) =>
        await Clients.All.Response_For_Screen_Three_Data(data);

    public override Task OnConnectedAsync()
    {
        Trace.WriteLine($"Client: {Context.ConnectionId} is connected !");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Trace.WriteLine($"Client: {Context.ConnectionId} is disconected !");
        _clientInfo.TryRemove(Context.ConnectionId, out _);
        return base.OnDisconnectedAsync(exception);
    }

    // Client sent ping to keep it alive
    public async Task Ping(string connectionId, HubRequestCommand screen)
    {
        ClientInfo oldValue = new();
        Trace.WriteLine($"{DateTime.Now} Ping from client: {connectionId} - {screen}");

        ClientInfo info = new ClientInfo
        {
            Request = screen,
            HeartBeat = DateTime.UtcNow
        };

        if (!_clientInfo.TryGetValue(connectionId, out oldValue))
        {
            var strScreen = screen.ToString();
            if(oldValue==null)
              await Groups.AddToGroupAsync(connectionId, strScreen);
        }

        _clientInfo.AddOrUpdate(connectionId, info, (key, oldValue) => info);
    }
}
