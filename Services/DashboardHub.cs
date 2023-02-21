using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Diagnostics;

public class DashboardHub: Hub<IDashboardHub> {
   public static readonly ConcurrentDictionary<string,HubRequestCommand> _clientScreens = new();

   public async Task SendDataToScreenOne(DtoScreenOneData data) =>
      await Clients.All.Response_For_Screen_One_Data(data);

   public async Task SendDataToScreenTwo(DtoScreenTwoData data) =>
      await Clients.All.Response_For_Screen_Two_Data(data);

   public async Task SendDataToScreenThree(DtoScreenThreeData data) =>
      await Clients.All.Response_For_Screen_Three_Data(data);

   public override Task OnConnectedAsync() {
      Trace.WriteLine($"Client: {Context.ConnectionId} is connected !");
      _clientScreens.TryAdd(Context.ConnectionId, HubRequestCommand.Unknown);
      return base.OnConnectedAsync();
   }

   public override Task OnDisconnectedAsync(Exception? exception) {
      Trace.WriteLine($"Client: {Context.ConnectionId} is disconected !");
      _clientScreens.TryRemove(Context.ConnectionId, out _);
      return base.OnDisconnectedAsync(exception);
   }

   public async Task SubscribeToScreen(HubRequestCommand screen) {
      HubRequestCommand oldValue;

      if(_clientScreens.TryGetValue(Context.ConnectionId,out oldValue)) {
         bool result = _clientScreens.TryUpdate(Context.ConnectionId, screen, oldValue);
         Trace.WriteLine($"Client: {Context.ConnectionId} is subscribing to screen : {screen} - {result}");
        
        var strScreen = screen.ToString();
        await Groups.AddToGroupAsync(Context.ConnectionId, strScreen);
      }
   }
}