
public class DtoScreenNameXClientIds {
   public HubRequestCommand Request { get; set; } 
   public List<string> ClientIds { get; set; } = new();
}

public class DtoTelemetryData
{ 
   public List<DtoScreenNameXClientIds> ScreenNameXClientIds { get; set; } = new();
}

