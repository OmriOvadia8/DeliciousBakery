using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game
{
    public class FoodManager : MonoBehaviour
    {
        private FoodData[] foods;
        private const int FOOD_COUNT = 10;

        void Start()
        {
            foods = new FoodData[FOOD_COUNT];

            foods[(int)FoodType.Burger] = new FoodData("Burger", 2, 10, 10);
            foods[(int)FoodType.Bread] = new FoodData("Bread", 1, 5, 50);
            foods[(int)FoodType.Candy] = new FoodData("Candy", 1, 3, 20);
            foods[(int)FoodType.Pizza] = new FoodData("Pizza", 3, 15, 200);
            foods[(int)FoodType.IceCream] = new FoodData("Ice Cream", 2, 8, 80);
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
        IceCream = 4
    }

}