using DB_Core;

namespace DB_Game
{
    public class DBTimeWrapItem : FoodDataAccess
    {
        private const int TWO_HOURS = 7200;
        public const int TWO_HOURS_PRICE = 1000;

        private const int FOUR_HOURS = 14400;
        public const int FOUR_HOURS_PRICE = 1800;

        private const int EIGHT_HOURS = 28800;
        public const int EIGHT_HOURS_PRICE = 3300;

        private int TimeWrap(int timePassed)
        {
            int totalReward = 0;

            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                var foodData = GetFoodData(i);

                if (foodData.IsBakerUnlocked)
                {
                    int reward = CalculateReward(timePassed, foodData);
                    totalReward += reward;
                }
            }

            return totalReward;
        }

        private int CalculateReward(int timePassed, FoodData foodData)
        {
            var bakerCookingTime = foodData.BakerCookingTime;
            int profit = foodData.Profit;

            int reward = (int)(timePassed / bakerCookingTime) * profit;

            return reward;
        }

        public void TwoHoursTimeWrap()
        {
            if (GameLogic.ScoreManager.TryUseScore(ScoreTags.PremiumCurrency, TWO_HOURS_PRICE))
            {
                int timeWrapReward = TimeWrap(TWO_HOURS);
                InvokeEvent(DBEventNames.AddCurrencyUpdate, timeWrapReward);
                InvokeEvent(DBEventNames.PremCurrencyUpdateUI, null);
                InvokeEvent(DBEventNames.CheckBuySkinButtonUI, null);
                InvokeEvent(DBEventNames.CheckBuyTimeWrapButtonsUI, null);
            }
        }

        public void FourHoursTimeWrap()
        {
            if (GameLogic.ScoreManager.TryUseScore(ScoreTags.PremiumCurrency, FOUR_HOURS_PRICE))
            {
                int timeWrapReward = TimeWrap(FOUR_HOURS);
                InvokeEvent(DBEventNames.AddCurrencyUpdate, timeWrapReward);
                InvokeEvent(DBEventNames.PremCurrencyUpdateUI, null);
                InvokeEvent(DBEventNames.CheckBuySkinButtonUI, null);
                InvokeEvent(DBEventNames.CheckBuyTimeWrapButtonsUI, null);
            }
        }

        public void EightHoursTimeWrap()
        {
            if (GameLogic.ScoreManager.TryUseScore(ScoreTags.PremiumCurrency, EIGHT_HOURS_PRICE))
            {
                int timeWrapReward = TimeWrap(EIGHT_HOURS);
                InvokeEvent(DBEventNames.AddCurrencyUpdate, timeWrapReward);
                InvokeEvent(DBEventNames.PremCurrencyUpdateUI, null);
                InvokeEvent(DBEventNames.CheckBuySkinButtonUI, null);
                InvokeEvent(DBEventNames.CheckBuyTimeWrapButtonsUI, null);
            }
        }
    }
}
