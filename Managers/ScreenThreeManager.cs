public class ScreenThreeManager : IScreenThreeManager
{
    DtoScreenThreeData result = new();
     
    public DtoScreenThreeData GetData()
    {
        result.Clock = $"Screen Three {DateTime.Now}";
        result.Counter += 100;
        return result;
    }
}
