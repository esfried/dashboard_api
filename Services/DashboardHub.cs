using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Diagnostics;

public class DashboardHub : Hub<IDashboardHub>
{
    public static readonly ConcurrentDictionary<string, HubRequestCommand> _clientScreens = new();
    public static readonly ConcurrentDictionary<string, DateTime> _clientHeartbeats = new();
    private System.Timers.Timer _heartbeatTimer;

    public DashboardHub()
    {
        _heartbeatTimer = new System.Timers.Timer(TimeSpan.FromSeconds(30).TotalMilliseconds);
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
            foreach (var connectionId in _clientHeartbeats.Keys)
            {
                if ((now - _clientHeartbeats[connectionId]).TotalSeconds > 60)
                {
                    Groups.RemoveFromGroupAsync(connectionId, _clientScreens[connectionId].ToString());
                    _clientScreens.TryRemove(connectionId, out _);
                    _clientHeartbeats.TryRemove(connectionId, out _);
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
        _clientScreens.TryAdd(Context.ConnectionId, HubRequestCommand.Unknown);
        _clientHeartbeats.TryAdd(Context.ConnectionId, DateTime.UtcNow);

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Trace.WriteLine($"Client: {Context.ConnectionId} is disconected !");
        _clientScreens.TryRemove(Context.ConnectionId, out _);

        _clientHeartbeats.TryRemove(Context.ConnectionId, out _);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SubscribeToScreen(HubRequestCommand screen)
    {
        HubRequestCommand oldValue;

        if (_clientScreens.TryGetValue(Context.ConnectionId, out oldValue))
        {
            bool result = _clientScreens.TryUpdate(Context.ConnectionId, screen, oldValue);
            Trace.WriteLine(
                $"Client: {Context.ConnectionId} is subscribing to screen : {screen} - {result}"
            );

            var strScreen = screen.ToString();
            await Groups.AddToGroupAsync(Context.ConnectionId, strScreen);
        }
    }

    public void OnReceiveHeartbeat()
    {
        _clientHeartbeats.TryUpdate(Context.ConnectionId, DateTime.UtcNow, DateTime.UtcNow);
    }
}
