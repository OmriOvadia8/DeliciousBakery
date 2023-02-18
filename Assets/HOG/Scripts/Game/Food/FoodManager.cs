using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game
{
    public class FoodManager : HOGLogicMonoBehaviour
    {
        private FoodData[] foods;
        private const int FOOD_COUNT = 10;
        private readonly Dictionary<int, HOGUpgradeableData> foodUpgradeMap = new();
        void Awake()
        {
            foods = new FoodData[FOOD_COUNT];

            foods[(int)FoodType.Burger] = new FoodData("Burger", 2, 10, 10, 0);
            foods[(int)FoodType.Bread] = new FoodData("Bread", 1, 5, 50, 1);
            foods[(int)FoodType.Candy] = new FoodData("Candy", 1, 3, 20, 2);
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

                Debug.Log(foodData.Level);
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