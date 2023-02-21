
public class DtoScreens {
   public HubRequestCommand Screen { get; set; } 
   public List<string> ClientIds { get; set; } = new();
}

public class DtoTelemetryData
{
    public string Name { get; } = "Telemetry";
    public string Clock { get; set; } = string.Empty;
    public int Counter { get; set; }
    public List<DtoScreens> Screens { get; set; } = new();
}

