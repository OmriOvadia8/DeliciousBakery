using UnityEngine;

namespace DB_Game
{
    public class DBPauseCurrencyManager : FoodDataAccess, IDBPauseCurrencyManager
    {
        [SerializeField] private int baseMaxReward = 500;

        public int PassedTimeFoodRewardCalc(int timePassed)
        {
            int totalReward = 0;
            int doubleProfit = DoubleProfitComponent.DoubleProfitMultiplier;

            // Calculate rewards for each idle food
            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                var foodData = GetFoodData(i);

                if (!foodData.IsFoodLocked && foodData.IsBakerUnlocked)
                {
                    int reward = CalculateReward(timePassed, foodData);
                    totalReward += reward;
                }
            }

            return totalReward * doubleProfit;
        }

        private int CalculateReward(int timePassed, FoodData foodData)
        {
            var bakerCookingTime = foodData.BakerCookingTime;
            int profit = foodData.Profit;

            int reward = (int)(timePassed / bakerCookingTime) * profit;
            int maxReward = CalculateMaxReward(foodData);

            return Mathf.Min(reward, maxReward);
        }

        private int CalculateMaxReward(FoodData foodData) => baseMaxReward * foodData.CookFoodMultiplier;
    }
}