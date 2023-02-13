using Core;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;


namespace Game
{
    public class CookingManager : HOGMonoBehaviour
    {
        public FoodData[] foods;
        private bool[] isFoodOnCooldown;
        private const int FOOD_COUNT = 10;

        private HOGMoneyHolder Currency => HOGGameLogic.Instance.PlayerMoney;
        private HOGScoreManager Score => HOGGameLogic.Instance.ScoreManager;


        void Start()
        {
            foods = new FoodData[FOOD_COUNT];
            isFoodOnCooldown = new bool[FOOD_COUNT];

            foods[(int)FoodType.Burger] = new FoodData("Burger", 1, 2, 10, 10);
            foods[(int)FoodType.Bread] = new FoodData("Bread", 1, 1, 5, 50);
            foods[(int)FoodType.Candy] = new FoodData("Candy", 1, 1, 3, 20);
            foods[(int)FoodType.Pizza] = new FoodData("Pizza", 1, 3, 15, 200);
            foods[(int)FoodType.IceCream] = new FoodData("Ice Cream", 1, 2, 8, 80);
            // ... and so on for the remaining food types
        }

        public void CookFood(int foodIndex)
        {
            if (isFoodOnCooldown[foodIndex])
            {
                return;
            }

            FoodData foodData = foods[foodIndex];
            float cookingTime = foodData.cookingTime;
            int profit = foodData.profit * foodData.level;
            Debug.Log(foodData.level);
            Debug.Log(foodData.levelUpCost);
            StartCoroutine(StartCooking(cookingTime, profit, foodIndex));
        }

        private IEnumerator StartCooking(float cookingTime, int profit, int foodIndex)
        {
            isFoodOnCooldown[foodIndex] = true;

            yield return new WaitForSeconds(cookingTime);

            HOGGameLogic.Instance.PlayerMoney.UpdateCurrency(profit);

            isFoodOnCooldown[foodIndex] = false;
        }

        public void LevelUpFood(int foodIndex)
        {
            FoodData foodData = foods[foodIndex];

            if (Score.TryUseScore(ScoreTags.GameCurrency, foodData.levelUpCost))
            {
                foodData.level++;
                foodData.levelUpCost = (int)(foodData.levelUpCost * 1.5f);
                foodData.cookingTime *= 0.8f;
                foodData.profit = (int)(foodData.profit * 1.5f);
                Currency.UpdateCurrency(-foodData.levelUpCost);
                Debug.Log("Kaching leveled up");
            }
            else
            {
                Debug.Log("NotEnough");
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
}