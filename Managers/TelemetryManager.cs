using System.Diagnostics;
using NuGet.Packaging;

public class TelemetryManager : ITelemetryManager
{
    DtoTelemetryData result = new();

    public DtoTelemetryData GetData()
    {
        result.Clock = DateTime.Now.ToString();
        result.Counter += 1;
        result.Screens.Clear();

        if (DashboardHub._clientInfo.Any())
        {
            var screens = DashboardHub._clientInfo.Select(x => x.Value.Request).Distinct();

            foreach (var screen in screens)
            {
                var newItem = new DtoScreens();
                newItem.Screen = screen;
                newItem.ClientIds.AddRange(DashboardHub._clientInfo.Where(x=>x.Value.Request==screen).Select(x => x.Key));
                result.Screens.Add(newItem);
            }
        }
        
        result.Screens = result.Screens.OrderBy(x=>x.Screen).ToList();
        return result;
    }
}
