using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;

//-----------------------------------------------------------------------------------------
// HubRequestCommand -> Command that is send by client on connect to hub
//-----------------------------------------------------------------------------------------

public enum HubRequestCommand {
  Unknown,
  Request_For_Screen_One_Data,
  Request_For_Screen_Two_Data,
  Request_For_Screen_Three_Data,
  Request_For_Telemetry_Data
}

//-----------------------------------------------------------------------------------------
// HubResponse -> At angular means which method will be used from server to update client
//-----------------------------------------------------------------------------------------
// Response_For_Screen_One_Data,
// Response_For_Screen_Two_Data,
// Response_For_Screen_Three_Data,
// Response_For_Telemetry_Data
//  
// They are used on DashboardHub like Clients.All.MethodName 
//      ex: Clients.All.Response_For_Screen_One_Data
//-----------------------------------------------------------------------------------------

public class DashboardDataService : BackgroundService
{
    private readonly ILogger<DashboardDataService> _logger;
    private readonly IHubContext<DashboardHub, IDashboardHub> _hubContext;
    private readonly IScreenOneManager _screenOneManager;
    private readonly IScreenTwoManager _screenTwoManager;
    private readonly IScreenThreeManager _screenThreeManager;
    private readonly ITelemetryManager _telemetryManager;

    public DashboardDataService(IHubContext<DashboardHub, IDashboardHub> hubContext,
    IScreenOneManager screenOneManager,
    IScreenTwoManager screenTwoManager,
    IScreenThreeManager screenThreeManager,
    ILogger<DashboardDataService> logger,
    ITelemetryManager telemetryManager)
    {
        _hubContext = hubContext;
        _screenOneManager = screenOneManager;
        _screenTwoManager = screenTwoManager;
        _screenThreeManager = screenThreeManager;
        _logger = logger;

        _logger.LogDebug("This is a debug message");
        _telemetryManager = telemetryManager;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (DashboardHub._clientScreens.Count > 0)
            {
                var items = _telemetryManager.GetData().ScreenNameXClientIds.ToList();
              
                foreach(var item in items) {
                    Trace.WriteLine($"Sending data into {item.Request} at {item.ClientIds.Count} client(s)");
                    
                    var request = item.Request.ToString();

                    switch(item.Request) {
                        case HubRequestCommand.Request_For_Screen_One_Data:
                            await _hubContext.Clients.Group(request).Response_For_Screen_One_Data(_screenOneManager.GetData());
                            break;
                       case HubRequestCommand.Request_For_Screen_Two_Data:
                            await _hubContext.Clients.Group(request).Response_For_Screen_Two_Data(_screenTwoManager.GetData());
                            break;
                       case HubRequestCommand.Request_For_Screen_Three_Data:
                            await _hubContext.Clients.Group(request).Response_For_Screen_Three_Data(_screenThreeManager.GetData());
                            break;
                       case HubRequestCommand.Request_For_Telemetry_Data:
                            await _hubContext.Clients.Group(request).Response_For_Telemetry_Data(_telemetryManager.GetData());
                            break;
                    }
                } 
            }

            await Task.Delay((int)System.TimeSpan.FromSeconds(1).TotalMilliseconds, stoppingToken); // aguarda 5 segundos
        }
    }
}
