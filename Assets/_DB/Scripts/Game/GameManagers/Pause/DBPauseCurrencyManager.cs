using System;

namespace DB_Game
{
    public class DBPauseCurrencyManager : FoodDataAccess, IDBPauseCurrencyManager
    {
        private const int MAX_OFFLINE_TIME = 15000;
        private const double NERF_REWARD = 0.5;

        public double PassedTimeFoodRewardCalc(int timePassed)
        {
            double totalReward = 0;
            int doubleProfit = DBDoubleProfitController.DoubleProfitMultiplier;

            int effectiveTimePassed = Math.Min(timePassed, MAX_OFFLINE_TIME);

            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                var foodData = GetFoodData(i);

                if (foodData.IsBakerUnlocked)
                {
                    double reward = CalculateReward(effectiveTimePassed, foodData);
                    totalReward += reward;
                }
            }

            return totalReward * doubleProfit / NERF_REWARD;
        }

        private double CalculateReward(int effectiveTimePassed, FoodData foodData)
        {
            var bakerCookingTime = foodData.BakerCookingTime;
            double profit = foodData.Profit;

            double reward = (double)(effectiveTimePassed / bakerCookingTime) * profit;

            return reward;
        }
    }
}
