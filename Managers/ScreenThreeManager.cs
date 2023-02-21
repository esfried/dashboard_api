public class ScreenThreeManager : IScreenThreeManager
{
    DtoScreenThreeData result = new();
     
    public DtoScreenThreeData GetData()
    {
        result.Clock = DateTime.Now.ToString();
        result.Counter += 100;
        return result;
    }
}
