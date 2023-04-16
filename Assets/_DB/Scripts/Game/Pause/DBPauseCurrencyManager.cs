using UnityEngine;

namespace DB_Game
{
    public static class DBPauseCurrencyManager
    {
        static int baseMaxReward = 500;

        static int totalReward = 0;
        static int maxReward = 0;

        public static int PassedTimeFoodRewardCalc(int timePassed)
        {
            totalReward = 0;
            maxReward = 0;

            // Calculate rewards for each idle food
            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                var foodData = DBFoodManager.GetFoodData(i);

                if (!foodData.IsFoodLocked && foodData.IsIdleFood)
                {
                    // Calculate reward for this food based on time passed
                    int reward = (int)(timePassed / foodData.BakerCookingTime) * foodData.Profit;

                    // Limit reward to avoid exploits
                    maxReward = baseMaxReward * foodData.CookFoodTimes;
                    reward = Mathf.Min(reward, maxReward);

                    // Add reward to total
                    totalReward += reward;
                }
            }

            return totalReward * DoubleProfitComponent.doubleProfitMultiplier;
        }

    }
}