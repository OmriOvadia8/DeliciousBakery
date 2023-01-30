public class HOGManager
{
    public static HOGManager Instance;

    public HOGManager()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public int GetNumber()
    {
        return 3;
    }
}