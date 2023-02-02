namespace Core
{
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
    }
}