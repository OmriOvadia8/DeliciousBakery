using DB_Core;
using TMPro;
using UnityEngine;

namespace DB_Game
{
    public class DBTimeWrapItem : FoodDataAccess
    {
        [SerializeField] TMP_Text[] CoinsAmountTimeWrapText;

        private const int TWO_HOURS = 7200;
        public const int TWO_HOURS_PRICE = 1000;

        private const int FOUR_HOURS = 14400;
        public const int FOUR_HOURS_PRICE = 1800;

        private const int EIGHT_HOURS = 28800;
        public const int EIGHT_HOURS_PRICE = 3300;

        private void OnEnable() => AddListener(DBEventNames.TimeWrapCoinsText, UpdateCoinsAmountTimeWrapUI);

        private void OnDisable() => RemoveListener(DBEventNames.TimeWrapCoinsText, UpdateCoinsAmountTimeWrapUI);

        private void Start() => UpdateCoinsAmountTimeWrapUI(null);

        private double TimeWrap(int timePassed)
        {
            double totalReward = 0;

            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                var foodData = GetFoodData(i);

                if (foodData.IsBakerUnlocked)
                {
                    double reward = CalculateReward(timePassed, foodData);
                    totalReward += reward;
                }
            }

            return totalReward;
        }

        private double CalculateReward(int timePassed, FoodData foodData)
        {
            var bakerCookingTime = foodData.BakerCookingTime;
            double profit = foodData.Profit;

            double reward = (double)(timePassed / bakerCookingTime) * profit;

            return reward;
        }

        public void TwoHoursTimeWrap()
        {
            if (GameLogic.ScoreManager.TryUseScore(ScoreTags.PremiumCurrency, TWO_HOURS_PRICE))
            {
                double timeWrapReward = TimeWrap(TWO_HOURS);
                InvokeTimeWrapEvents(timeWrapReward);
            }
        }

        public void FourHoursTimeWrap()
        {
            if (GameLogic.ScoreManager.TryUseScore(ScoreTags.PremiumCurrency, FOUR_HOURS_PRICE))
            {
                double timeWrapReward = TimeWrap(FOUR_HOURS);
                InvokeTimeWrapEvents(timeWrapReward);
            }
        }

        public void EightHoursTimeWrap()
        {
            if (GameLogic.ScoreManager.TryUseScore(ScoreTags.PremiumCurrency, EIGHT_HOURS_PRICE))
            {
                double timeWrapReward = TimeWrap(EIGHT_HOURS);
                InvokeTimeWrapEvents(timeWrapReward);
            }
        }

        private void UpdateCoinsAmountTimeWrapUI(object obj)
        {
            CoinsAmountTimeWrapText[0].text = TimeWrap(TWO_HOURS).ToReadableNumber();
            CoinsAmountTimeWrapText[1].text = TimeWrap(FOUR_HOURS).ToReadableNumber();
            CoinsAmountTimeWrapText[2].text = TimeWrap(EIGHT_HOURS).ToReadableNumber();
        }

        private void InvokeTimeWrapEvents(double timeWrapReward)
        {
            InvokeEvent(DBEventNames.AddCurrencyUpdate, timeWrapReward);
            InvokeEvent(DBEventNames.PremCurrencyUpdateUI, null);
            InvokeEvent(DBEventNames.CheckBuySkinButtonUI, null);
            InvokeEvent(DBEventNames.CheckBuyTimeWrapButtonsUI, null);
            InvokeEvent(DBEventNames.BuyButtonsCheck, null);
            InvokeEvent(DBEventNames.PlaySound, SoundEffectType.ButtonClick);
        }
    }
}
