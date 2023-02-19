using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game
{
    public class FoodManager : HOGLogicMonoBehaviour
    {
        private FoodData[] foods;
        private const int FOOD_COUNT = 10; // total food types count in game
        private const float PROFIT_INCREASE = 1.1f; // increasing the food's profit by 10% each upgrade
        private const float COST_INCREASE = 1.15f; // increasing the upgrade's cost by 20% each upgrade
 
        void Awake()
        {
            foods = new FoodData[FOOD_COUNT];

            foods[(int)FoodType.Burger] = new FoodData(2, 30, 100);
            foods[(int)FoodType.Bread] = new FoodData(1, 23, 72);
            foods[(int)FoodType.Candy] = new FoodData(1, 15, 40);
            foods[(int)FoodType.Pizza] = new FoodData(3, 15, 200);
            foods[(int)FoodType.IceCream] = new FoodData(2, 8, 80);
            foods[(int)FoodType.Donut] = new FoodData(2, 8, 80);
            foods[(int)FoodType.Cookie] = new FoodData(2, 8, 80);
            foods[(int)FoodType.Cupcake] = new FoodData(2, 8, 80);
            foods[(int)FoodType.Cake] = new FoodData(2, 8, 80);
            foods[(int)FoodType.Brownie] = new FoodData(2, 8, 80);
        }

        private void Start()
        {
            for (int i = 0; i < FOOD_COUNT; i++)
            {
                AddNewFoodItem(i);
                InvokeEvent(HOGEventNames.OnUpgraded, i);
            }
        }

        public void AddNewFoodItem(int foodID)
        {
            GameLogic.UpgradeManager.PlayerUpgradeInventoryData.Upgradeables.Add(new HOGUpgradeableData
            {
                upgradableTypeID = UpgradeablesTypeID.Food,
                CurrentLevel = 1,
                foodID = foodID
            });
        }

        public void UpgradeFood(int foodID)
        {
            var foodData = foods[foodID];
            var upgradeableType = UpgradeablesTypeID.Food;

            GameLogic.UpgradeManager.UpgradeItemByID(upgradeableType, foodID);

            foodData.Profit = (int)(foodData.Profit * PROFIT_INCREASE);
            foodData.LevelUpCost = (int)(foodData.LevelUpCost * COST_INCREASE);

            InvokeEvent(HOGEventNames.OnUpgraded, foodID);
            Debug.Log(GameLogic.UpgradeManager.GetUpgradeableByID(upgradeableType, foodID).CurrentLevel);
        }

        public FoodData GetFoodData(int foodIndex)
        {
            return foods[foodIndex];
        }

        public bool IsFoodOnCooldown(int foodIndex)
        {
            return foods[foodIndex].IsOnCooldown;
        }

        public void SetFoodOnCooldown(int foodIndex, bool value)
        {
            foods[foodIndex].IsOnCooldown = value;
        }
    }

    public enum FoodType
    {
        Burger = 0,
        Bread = 1,
        Candy = 2,
        Pizza = 3,
        IceCream = 4,
        Donut = 5,
        Cookie = 6,
        Cupcake = 7,
        Cake = 8,
        Brownie = 9
    }
}