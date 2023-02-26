using Core;
using System;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class FoodManager : HOGLogicMonoBehaviour
    {
        [SerializeField] HOGMoneyHolder moneyHolder;
        private FoodDataCollection foods; // fooddatacollection sub class in FoodData.cs (array of foods)

        public const int FOOD_COUNT = 10; // total food types count in game
        private const float PROFIT_INCREASE = 1.1f; // increasing the food's profit by 10% each upgrade
        private const float COST_INCREASE = 1.15f; // increasing the upgrade's cost by 15% each upgrade
        private const string FOOD_CONFIG_PATH = "food_data"; // the name of the config file to load

        private void OnEnable()
        {
            // loading the latest food data list stats
            HOGManager.Instance.SaveManager.Load<FoodDataCollection>(data =>
            {
                if (data != null)
                {
                    foods = data;
                    Debug.Log("Loaded upgrade cost: " + data.Foods[0].UpgradeCost);

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
                AddNewFoodItem(i);
                InvokeEvent(HOGEventNames.OnUpgraded, i);
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
                foodData.Profit = (int)(foodData.Profit * PROFIT_INCREASE);
                foodData.UpgradeCost = (int)(foodData.UpgradeCost * COST_INCREASE);
                

                InvokeEvent(HOGEventNames.OnUpgraded, foodID);
                
                HOGManager.Instance.SaveManager.Save(foods); // Saving the current food data list stats
                moneyHolder.UpdateCurrency(moneyHolder.startingCurrency);
            }

            Debug.Log(GameLogic.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.Food, foodID).CurrentLevel);
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
    }
}
