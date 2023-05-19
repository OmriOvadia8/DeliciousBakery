using System;
using UnityEngine;

namespace DB_Game
{
    public class DBPauseCurrencyManager : FoodDataAccess, IDBPauseCurrencyManager
    {
        [SerializeField] private double baseMaxReward = 500;

        public double PassedTimeFoodRewardCalc(int timePassed)
        {
            double totalReward = 0;
            int doubleProfit = DBDoubleProfitController.DoubleProfitMultiplier;

            // Calculate rewards for each idle food
            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                var foodData = GetFoodData(i);

                if (!foodData.IsFoodLocked && foodData.IsBakerUnlocked)
                {
                    double reward = CalculateReward(timePassed, foodData);
                    totalReward += reward;
                }
            }

            return totalReward * doubleProfit;
        }

        private double CalculateReward(int timePassed, FoodData foodData)
        {
            var bakerCookingTime = foodData.BakerCookingTime;
            double profit = foodData.Profit;

            double reward = (double)(timePassed / bakerCookingTime) * profit;
            double maxReward = CalculateMaxReward(foodData);

            return Math.Min(reward, maxReward);
        }

        private double CalculateMaxReward(FoodData foodData) => baseMaxReward * foodData.CookFoodMultiplier;
    }
}