public interface IDashboardHub
{
    Task SubscribeToScreen(HubRequestCommand screenId);
    Task Response_For_Screen_One_Data(DtoScreenOneData data);
    Task Response_For_Screen_Two_Data(DtoScreenTwoData data);
    Task Response_For_Screen_Three_Data(DtoScreenThreeData data);
    Task Response_For_Telemetry_Data(DtoTelemetryData data);
}