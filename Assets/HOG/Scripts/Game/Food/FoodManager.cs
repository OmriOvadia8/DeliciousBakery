using Core;
using System;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class FoodManager : HOGLogicMonoBehaviour
    {
        [SerializeField] HOGMoneyHolder moneyHolder;
        private FoodDataCollection foods;
        public const int FOOD_COUNT = 10; // total food types count in game
        private const float PROFIT_INCREASE = 1.1f; // increasing the food's profit by 10% each upgrade
        private const float COST_INCREASE = 1.15f; // increasing the upgrade's cost by 15% each upgrade
        private const string FOOD_CONFIG_PATH = "food_data"; // the name of the config file to load

        private void Awake()
        {
            
            HOGManager.Instance.SaveManager.Load<FoodDataCollection>(data =>
            {
                if (data != null)
                {
                    foods = data;
                }
                else
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

            if (initialLevel < GameLogic.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.Food, foodID).CurrentLevel)
            {
                foodData.Profit = (int)(foodData.Profit * PROFIT_INCREASE);
                foodData.UpgradeCost = (int)(foodData.UpgradeCost * COST_INCREASE);

                InvokeEvent(HOGEventNames.OnUpgraded, foodID);
                
                HOGManager.Instance.SaveManager.Save(foods);
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
//// Load saved food data
//GameLogic.FoodDataManager.LoadFoods((foodData) =>
//{
//    if (foodData != null)
//    {
//        // If saved data exists, update the Foods dictionary with the loaded data
//        GameLogic.FoodDataManager.Foods = foodData;
//    }
//    else
//    {
//        // If no saved data exists, initialize the Foods dictionary with default data
//        GameLogic.FoodDataManager.Foods = new Dictionary<int, FoodData>();
//    }
//});