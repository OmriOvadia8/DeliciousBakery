using DB_Core;
using System.Linq;
using UnityEngine;

namespace DB_Game
{
    public class DBFoodManager : DBLogicMonoBehaviour
    {
        [SerializeField] DBCurrencyManager currencyManager;
        [SerializeField] DBCookingManager cookingManager;
        [SerializeField] UIManager uiManager;
        [SerializeField] GameObject[] LockedFoodBars;
        [SerializeField] GameObject[] LockedBakersBars;

        public static FoodDataCollection foods; // fooddatacollection sub class in FoodData.cs (array of foods)

        public bool[] isIdleUnlocked = new bool[FOOD_COUNT];

        public const int FOOD_COUNT = 10; // total food types count in game
        private const float PROFIT_INCREASE = 1.1f; // increasing the food's profit by 10% each upgrade
        private const float COST_INCREASE = 1.15f; // increasing the upgrade's cost by 15% each upgrade
        private const float BAKER_COST_INCREASE = 1.3f;
        private const string FOOD_CONFIG_PATH = "food_data"; // the name of the config file to load

        private void OnEnable()
        {
            // loading the latest food data list stats
            DBManager.Instance.SaveManager.Load<FoodDataCollection>(data =>
            {
                if (data != null)
                {
                    foods = data;
                }
                else // or default if there isnt one
                    DBManager.Instance.ConfigManager.GetConfigAsync<FoodDataCollection>(FOOD_CONFIG_PATH, OnConfigLoaded);
            });
        }

        private void Start()
        {
            // Add all foods to the upgradeables list and update UI
            for (int i = 0; i < FOOD_COUNT; i++)
            {
                var foodData = GetFoodData(i);

                AddNewFoodItem(i);

                InvokeEvent(DBEventNames.OnUpgraded, i);
                InvokeEvent(DBEventNames.OnHired, i);
                InvokeEvent(DBEventNames.OnLearnRecipe, i);

                if (foodData.IsFoodLocked == false) // setting the locked/unlocked saved food on launch
                {
                    LockedFoodBars[i].SetActive(false);
                    LockedBakersBars[i].SetActive(false);
                }
            }
        }

        private void OnConfigLoaded(FoodDataCollection configData)
        {
            foods = configData;
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

        public static FoodData GetFoodData(int foodID)
        {
            var foodData = foods?.Foods.FirstOrDefault(fd => fd.Index == foodID);

            if (foodData == null)
            {
                Debug.LogError("Invalid food ID: " + foodID);
            }

            return foodData;
        }

        public void LearnRecipe(int foodID)
        {
            var foodData = GetFoodData(foodID);

            if (DBGameLogic.Instance.ScoreManager.TryUseScore(ScoreTags.GameCurrency, foodData.UnlockCost))
            {
                foodData.IsFoodLocked = false;
                LockedFoodBars[foodID].SetActive(false);
                LockedBakersBars[foodID].SetActive(false);

                InvokeEvent(DBEventNames.OnLearnRecipeSpentToast, foodID);

                DBManager.Instance.SaveManager.Save(foods);
            }
            else
            {
                DBDebug.LogException("Not enough money to unlock recipe!");
            }
        }

        public void UpgradeFood(int foodID)
        {
            if (foods == null)
            {
                DBDebug.LogException("Food data not loaded");
                return;
            }

            var foodData = GetFoodData(foodID);

            if (foodData == null)
            {
                DBDebug.LogException("Invalid food ID: " + foodID);
                return;
            }

            int initialLevel = GameLogic.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.Food, foodID).CurrentLevel;
            int initialUpgradeCost = foodData.UpgradeCost;

            GameLogic.UpgradeManager.UpgradeItemByID(UpgradeablesTypeID.Food, foodID, ScoreTags.GameCurrency, initialUpgradeCost);

            if (initialLevel < GameLogic.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.Food, foodID).CurrentLevel) // checks if the item leveled up and if so increases stats
            {
                InvokeEvent(DBEventNames.OnUpgradeMoneySpentToast, foodID);

                foodData.Profit = (int)(foodData.Profit * PROFIT_INCREASE);
                foodData.UpgradeCost = (int)(foodData.UpgradeCost * COST_INCREASE);

                InvokeEvent(DBEventNames.OnUpgraded, foodID);

                DBManager.Instance.SaveManager.Save(foods); // Saving the current food data list stats
            }

            DBDebug.Log(GameLogic.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.Food, foodID).CurrentLevel);
        }

        public void UnlockOrUpgradeIdleFood(int foodIndex)
        {
            var foodData = GetFoodData(foodIndex);

            if (DBGameLogic.Instance.ScoreManager.TryUseScore(ScoreTags.GameCurrency, foodData.HireCost))
            {
                InvokeEvent(DBEventNames.OnHireMoneySpentToast, foodIndex);
                foodData.IsIdleFood = true;
                cookingManager.AutoCookFood(foodIndex);
                foodData.HireCost = (int)(foodData.HireCost * BAKER_COST_INCREASE);
                if (foodData.BakersCount % 3 == 0) // check if baker count is a multiple of 3
                {
                    foodData.CookFoodTimes++;
                }
                foodData.BakersCount++;
                InvokeEvent(DBEventNames.OnHired, foodIndex);
            }

            else
            {
                DBDebug.LogException("Failed to unlock/update idle");
            }

            DBManager.Instance.SaveManager.Save(foods);
        }
    }
}