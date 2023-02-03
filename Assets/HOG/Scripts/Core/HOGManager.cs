namespace Core
{
    public class HOGManager
    {
        public static HOGManager Instance;

        public HOGEventsManager EventsManager;
        public HOGFactoryManager FactoryManager;
        public HOGPoolManager PoolManager;

        public HOGManager()
        {
            if (Instance != null)
            {
                return;
            }

            Instance = this;

            EventsManager = new HOGEventsManager();
            FactoryManager = new HOGFactoryManager();
            PoolManager = new HOGPoolManager();
        }
    }
}