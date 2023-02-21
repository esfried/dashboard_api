public class ScreenOneManager : IScreenOneManager {
    DtoScreenOneData result = new();
    
    public DtoScreenOneData GetData()
    {
        result.Clock = $"Screen One {DateTime.Now}";
        return result;
    }
}
