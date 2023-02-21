public class ScreenOneManager : IScreenOneManager {
    DtoScreenOneData result = new();
    
    public DtoScreenOneData GetData()
    {
        result.Clock = DateTime.Now.ToString();
        return result;
    }
}
