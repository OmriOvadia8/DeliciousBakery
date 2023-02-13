using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game
{
    public class FoodManager : MonoBehaviour
    {
        public FoodData[] foods;
        private const int FOOD_COUNT = 10;
        private bool[] isFoodOnCooldown;
        void Start()
        {
            foods = new FoodData[FOOD_COUNT];
            isFoodOnCooldown = new bool[FOOD_COUNT];

            foods[(int)FoodType.Burger] = new FoodData("Burger", 1, 2, 10, 10);
            foods[(int)FoodType.Bread] = new FoodData("Bread", 1, 1, 5, 50);
            foods[(int)FoodType.Candy] = new FoodData("Candy", 1, 1, 3, 20);
            foods[(int)FoodType.Pizza] = new FoodData("Pizza", 1, 3, 15, 200);
            foods[(int)FoodType.IceCream] = new FoodData("Ice Cream", 1, 2, 8, 80);
            


            for (int i = 0; i < FOOD_COUNT; i++)
            {
                isFoodOnCooldown[i] = false;
            }

        }

        public FoodData GetFoodData(int foodIndex)
        {
            return foods[foodIndex];
        }

        public bool IsFoodOnCooldown(int foodIndex)
        {
            return isFoodOnCooldown[foodIndex];
        }

        public void SetFoodOnCooldown(int foodIndex, bool value)
        {
            isFoodOnCooldown[foodIndex] = value;
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