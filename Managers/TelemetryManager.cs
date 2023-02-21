using System.Diagnostics;

public class TelemetryManager : ITelemetryManager
{
    DtoTelemetryData result = new();

    public DtoTelemetryData GetData()
    {
        result.ScreenNameXClientIds.Clear();

        if (DashboardHub._clientScreens.Count > 0)
        {
            var groups = DashboardHub._clientScreens.GroupBy(pair => pair.Value);

            foreach (var group in groups)
            {
                var newItem = new DtoScreenNameXClientIds();
                newItem.Request = group.Key;
                
                foreach (var pair in group)
                    newItem.ClientIds.Add(pair.Key);
                
                result.ScreenNameXClientIds.Add(newItem);
            }
        }
        
        result.ScreenNameXClientIds = result.ScreenNameXClientIds.OrderBy(x=>x.Request).ToList();
        return result;
    }
}
