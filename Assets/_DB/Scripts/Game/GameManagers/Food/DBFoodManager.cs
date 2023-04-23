using DB_Core;
using System;
using System.Linq;
using UnityEngine;

namespace DB_Game
{
    public class DBFoodManager : DBLogicMonoBehaviour
    {
        [SerializeField] GameObject[] LockedFoodBars;
        [SerializeField] GameObject[] LockedBakersBars;

        public static FoodDataCollection Foods;
        public const int FOOD_COUNT = 10;
        private const string FOOD_CONFIG_PATH = "food_data";

        private IFoodDataRepository foodDataRepository;
        private IFoodUpgrader foodUpgrader;
        private IFoodUnlocker foodUnlocker;
        private IBakerFoodManager bakerFoodManager;

        private void OnEnable()
        {
            DBManager.Instance.SaveManager.Load<FoodDataCollection>(data =>
            {
                if (data != null)
                {
                    Foods = data;
                    Debug.Log("Food data loaded successfully");
                    InitializeManagers();
                }
                else
                {
                    DBManager.Instance.ConfigManager.GetConfigAsync<FoodDataCollection>(FOOD_CONFIG_PATH, OnConfigLoaded);
                    Debug.Log("else Food data loaded successfully");
                }
            });
        }

        private void InitializeManagers()
        {
            foodDataRepository = new FoodDataRepository(Foods);
            foodUpgrader = new FoodUpgrader(foodDataRepository, DBManager.Instance);
            foodUnlocker = new FoodUnlocker(foodDataRepository, DBManager.Instance);
            bakerFoodManager = new BakerFoodManager(foodDataRepository, DBManager.Instance);
        }

        private void OnConfigLoaded(FoodDataCollection configData)
        {
            Foods = configData;
            Debug.Log("Food data loaded successfully onconfigloaded");
            InitializeManagers();
        }

        private void Start()
        {
            for (int i = 0; i < FOOD_COUNT; i++)
            {
                var foodData = foodDataRepository.GetFoodData(i);

                AddNewFoodItem(i);

                InvokeEvent(DBEventNames.OnUpgraded, i);
                InvokeEvent(DBEventNames.OnHired, i);
                InvokeEvent(DBEventNames.OnLearnRecipe, i);

                if (foodData.IsFoodLocked)
                {
                   InvokeEvent(DBEventNames.FoodBarLocked, i);
                }
            }
        }

        private void AddNewFoodItem(int foodID)
        {
            GameLogic.UpgradeManager.PlayerUpgradeInventoryData.Upgradeables.Add(new DBUpgradeableData
            {
                upgradableTypeID = UpgradeablesTypeID.Food,
                CurrentLevel = 1,
                foodID = foodID
            });
        }

        public void LearnRecipe(int foodID)
        {
            foodUnlocker.LearnRecipe(foodID);
        }

        public void UpgradeFood(int foodID)
        {
            foodUpgrader.UpgradeFood(foodID);
        }

        public void UnlockOrUpgradeIdleFood(int foodIndex)
        {
            bakerFoodManager.UnlockOrUpgradeIdleFood(foodIndex);
        }
    }
}