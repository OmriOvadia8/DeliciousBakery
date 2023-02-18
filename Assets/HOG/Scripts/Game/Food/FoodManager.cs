using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game
{
    public class FoodManager : HOGLogicMonoBehaviour
    {
        public FoodData[] foods;
        private const int FOOD_COUNT = 10;
        private readonly Dictionary<int, HOGUpgradeableData> foodUpgradeMap = new();
        private const float profitIncrease = 1.1f;
        private const float levelUpCostIncrease = 1.2f;

        void Awake()
        {
            foods = new FoodData[FOOD_COUNT];

            foods[(int)FoodType.Burger] = new FoodData("Burger", 2, 20, 100, 0);
            foods[(int)FoodType.Bread] = new FoodData("Bread", 1, 14, 30, 1);
            foods[(int)FoodType.Candy] = new FoodData("Candy", 1, 12, 20, 2);
            foods[(int)FoodType.Pizza] = new FoodData("Pizza", 3, 15, 200, 3);
            foods[(int)FoodType.IceCream] = new FoodData("Ice Cream", 2, 8, 80, 4);

            foreach (var foodItem in foods)
            {
                if (foodItem != null)
                {
                    foodUpgradeMap[foodItem.FoodID] = new HOGUpgradeableData
                    {
                        upgradableTypeID = UpgradeablesTypeID.Food,
                        CurrentLevel = 1
                    };
                }
            }
        }

        private void Start()
        {
            for (int i = 0; i < FOOD_COUNT; i++)
            {
                InvokeEvent(HOGEventNames.OnUpgraded, i);
                Debug.Log(foods[i].LevelUpCost); 
            }
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

        public void UpgradeFood(int foodID)
        {
            if (foodUpgradeMap.TryGetValue(foodID, out var upgradeable))
            {
                GameLogic.UpgradeManager.UpgradeItemByID(upgradeable.upgradableTypeID);

                var foodData = foods[foodID];

                upgradeable.CurrentLevel++;
                foodData.Level = upgradeable.CurrentLevel;
                foodData.Profit = (int)(foodData.Profit * profitIncrease);
                foodData.LevelUpCost = (int)(foodData.LevelUpCost * levelUpCostIncrease);

                InvokeEvent(HOGEventNames.OnUpgraded, foodID);
            }
        }
    }

    public enum FoodType
    {
        Burger = 0,
        Bread = 1,
        Candy = 2,
        Pizza = 3,
        IceCream = 4
    }

}