using System;
using DB_Core;

namespace DB_Game
{
    public class DBGameLogic : IDBBaseManager
    {
        public static DBGameLogic Instance;

        public DBScoreManager ScoreManager;
        public DBUpgradeManager UpgradeManager;
        public DBStoreManager StoreManager;
        public FoodDataRepository FoodDataRepository;

        public DBGameLogic()
        {
            if (Instance != null)
            {
                return;
            }

            Instance = this;
        }

        public void LoadManager(Action onComplete)
        {
            ScoreManager = new DBScoreManager();
            UpgradeManager = new DBUpgradeManager();
            StoreManager = new DBStoreManager();

            FoodDataCollection foodDataCollection = DBFoodManager.Foods;
            FoodDataRepository = new FoodDataRepository(foodDataCollection);

            onComplete.Invoke();
        }
    }
}