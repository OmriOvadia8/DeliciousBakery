using Core;
using System;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class FoodManager : HOGLogicMonoBehaviour
    {
        [SerializeField] HOGMoneyHolder moneyHolder;
        [SerializeField] CookingManager cookingManager;
        [SerializeField] UIManager uiManager;
        [SerializeField] GameObject[] LockedFoodBars;
        [SerializeField] GameObject[] LockedBakersBars;

        public FoodDataCollection foods; // fooddatacollection sub class in FoodData.cs (array of foods)

        public bool[] isIdleUnlocked = new bool[FOOD_COUNT];

        public const int FOOD_COUNT = 10; // total food types count in game
        private const float PROFIT_INCREASE = 1.1f; // increasing the food's profit by 10% each upgrade
        private const float COST_INCREASE = 1.15f; // increasing the upgrade's cost by 15% each upgrade
        private const float BAKER_COST_INCREASE = 1.3f;
        private const string FOOD_CONFIG_PATH = "food_data"; // the name of the config file to load

        private void OnEnable()
        {
            // loading the latest food data list stats
            HOGManager.Instance.SaveManager.Load<FoodDataCollection>(data =>
            {
                if (data != null)
                {
                    foods = data;
                }
                else // or default if there isnt one
                    HOGManager.Instance.ConfigManager.GetConfigAsync<FoodDataCollection>(FOOD_CONFIG_PATH, OnConfigLoaded);
            });
        }

        private void Start()
        {
            // Add all foods to the upgradeables list and update UI
            for (int i = 0; i < FOOD_COUNT; i++)
            {
                var foodData = GetFoodData(i);

                AddNewFoodItem(i);

                InvokeEvent(HOGEventNames.OnUpgraded, i);
                InvokeEvent(HOGEventNames.OnHired, i);
                InvokeEvent(HOGEventNames.OnLearnRecipe, i);

                

                if (foodData.IsFoodLocked == false) // setting the locked/unlocked saved food on launch
                {
                    LockedFoodBars[i].SetActive(false);
                    LockedBakersBars[i].SetActive(false);
                }

                if (foodData.IsIdleFood == true)
                {
                    foodData.IsAutoOnCooldown = true;
                   // cookingManager.AutoCookFoodAfterOffline(i);
                }
            }
        }

        private void OnConfigLoaded(FoodDataCollection configData)
        {
            foods = configData;
        }

        private void AddNewFoodItem(int foodID)
        {
            GameLogic.UpgradeManager.PlayerUpgradeInventoryData.Upgradeables.Add(new HOGUpgradeableData
            {
                upgradableTypeID = UpgradeablesTypeID.Food,
                CurrentLevel = 1,
                foodID = foodID
            });
        }


        public FoodData GetFoodData(int foodID)
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

            if (HOGGameLogic.Instance.ScoreManager.TryUseScore(ScoreTags.GameCurrency, foodData.UnlockCost))
            {
                foodData.IsFoodLocked = false;
                LockedFoodBars[foodID].SetActive(false);
                LockedBakersBars[foodID].SetActive(false);

                InvokeEvent(HOGEventNames.OnLearnRecipeSpentToast, foodID);

                HOGManager.Instance.SaveManager.Save(foods);
            }
            else
            {
                HOGDebug.LogException("Not enough money to unlock recipe!");
            }
        }

        public void UpgradeFood(int foodID)
        {
            if (foods == null)
            {
                Debug.LogError("Food data not loaded");
                return;
            }

            var foodData = GetFoodData(foodID);

            if (foodData == null)
            {
                Debug.LogError("Invalid food ID: " + foodID);
                return;
            }

            int initialLevel = GameLogic.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.Food, foodID).CurrentLevel;
            int initialUpgradeCost = foodData.UpgradeCost;

            GameLogic.UpgradeManager.UpgradeItemByID(UpgradeablesTypeID.Food, foodID, ScoreTags.GameCurrency, initialUpgradeCost);

            if (initialLevel < GameLogic.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.Food, foodID).CurrentLevel) // checks if the item leveled up and if so increases stats
            {
                InvokeEvent(HOGEventNames.OnUpgradeMoneySpentToast, foodID);

                foodData.Profit = (int)(foodData.Profit * PROFIT_INCREASE);
                foodData.UpgradeCost = (int)(foodData.UpgradeCost * COST_INCREASE);

                InvokeEvent(HOGEventNames.OnUpgraded, foodID);

                HOGManager.Instance.SaveManager.Save(foods); // Saving the current food data list stats
            }

            Debug.Log(GameLogic.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.Food, foodID).CurrentLevel);
        }

        public void UnlockOrUpgradeIdleFood(int foodIndex)
        {
            var foodData = GetFoodData(foodIndex);

            if (HOGGameLogic.Instance.ScoreManager.TryUseScore(ScoreTags.GameCurrency, foodData.HireCost))
            {
                InvokeEvent(HOGEventNames.OnHireMoneySpentToast, foodIndex);

                foodData.IsIdleFood = true;

                cookingManager.AutoCookFood(foodIndex);

                foodData.HireCost = (int)(foodData.HireCost * BAKER_COST_INCREASE);

                if (foodData.BakersCount % 3 == 0) // check if baker count is a multiple of 3
                {
                    foodData.CookFoodTimes++;
                }

                foodData.BakersCount++;

                InvokeEvent(HOGEventNames.OnHired, foodIndex);
            }
            else
            {
                HOGDebug.LogException("something's wrong here");
            }

            HOGManager.Instance.SaveManager.Save(foods);
        }

        public bool IsFoodOnCooldown(int foodID)
        {
            var foodData = GetFoodData(foodID);

            if (foodData == null)
            {
                return false;
            }

            return foodData.IsOnCooldown;
        }

        public void SetFoodOnCooldown(int foodID, bool value)
        {
            if (foods != null)
            {
                FoodData foodData = GetFoodData(foodID);

                if (foodData != null)
                {
                    foodData.IsOnCooldown = value;
                }
                else
                {
                    Debug.LogError("Invalid food ID: " + foodID);
                }
            }
            else
            {
                Debug.LogError("Food data not loaded");
            }
        }

        public bool IsAutoFoodOnCooldown(int foodID)
        {
            var foodData = GetFoodData(foodID);

            if (foodData == null)
            {
                return false;
            }

            return foodData.IsAutoOnCooldown;
        }

        public void SetAutoFoodOnCooldown(int foodID, bool value)
        {
            if (foods != null)
            {
                FoodData foodData = GetFoodData(foodID);

                if (foodData != null)
                {
                    foodData.IsAutoOnCooldown = value;
                }
                else
                {
                    Debug.LogError("Invalid food ID: " + foodID);
                }
            }
            else
            {
                Debug.LogError("Food data not loaded");
            }
        }
    }
}