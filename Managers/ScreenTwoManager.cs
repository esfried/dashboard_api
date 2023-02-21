public class ScreenTwoManager : IScreenTwoManager {
    DtoScreenTwoData result = new();
    
    public DtoScreenTwoData GetData()
    {
        ++result.Counter;
        return result;
    }
}
