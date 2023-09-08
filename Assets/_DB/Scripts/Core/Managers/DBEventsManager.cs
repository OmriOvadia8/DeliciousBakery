using System;
using System.Collections.Generic;

namespace DB_Core
{
    public class DBEventsManager
    {
        private readonly Dictionary<DBEventNames, List<Action<object>>> activeListeners = new();

        public void AddListener(DBEventNames eventName, Action<object> onGameStart)
        {
            if (activeListeners.TryGetValue(eventName, out var listOfEvents))
            {
                listOfEvents.Add(onGameStart);
                return;
            }

            activeListeners.Add(eventName, new List<Action<object>> { onGameStart });
        }

        public void RemoveListener(DBEventNames eventName, Action<object> onGameStart)
        {
            if (activeListeners.TryGetValue(eventName, out var listOfEvents))
            {
                listOfEvents.Remove(onGameStart);

                if (listOfEvents.Count <= 0)
                {
                    activeListeners.Remove(eventName);
                }
            }
        }

        public void InvokeEvent(DBEventNames eventName, object obj)
        {
            if (activeListeners.TryGetValue(eventName, out var listOfEvents))
            {
                foreach (var action in listOfEvents)
                {
                    action.Invoke(obj);
                }
            }
        }
    }

    public enum DBEventNames
    {
        OnCurrencySet,
        OnUpgradeTextUpdate,
        MoneyToastOnCook,
        MoneyToastOnAutoCook,
        OnUpgradeMoneySpentToast,
        OnHireMoneySpentToast,
        OnHiredTextUpdate,
        OnPause,
        OfflineTimeRefreshed,
        OnLearnRecipe,
        OnLearnRecipeSpentToast,
        CurrencyUpdateUI,
        OnPremCurrencySet,
        PremCurrencyUpdateUI,
        DeviceAppearAnimation,
        AddCurrencyUpdate,
        StartBakerCooking,
        BuyButtonsCheck,
        CookButtonAlphaOn,
        CookButtonAlphaOff,
        BakerParticles,
        LearnParticles,
        FoodBarReveal,
        FoodBarLocked,
        UpgradeParticles,
        CookParticles,
        CookFoodButtonCheck,
        CheckCookedAchievement,
        CheckHiredAchievement,
        CurrentMakeFoodAchievementStatus,
        CurrentHireBakerAchievementStatus,
        AddStarsUpdate,
        MakeFoodProgressUpdate,
        HireBakerProgressUpdate,
        CheckBuySkinButtonUI,
        BuySkinButtonVisibility,
        CheckBuyTimeWrapButtonsUI,
        PlaySound,
        AchievementPing,
        BakerPing,
        SlimeAction,
        TimeWrapCoinsText,
        Match3ScoreTextIncrease,
        Match3MovesTextUpdate,
        Match3GameOverScreen,
        Match3ScoreToast,
        Match3RestartButtonVisibility,
        Match3CanvasOrder,
        Match3GameEndText,
        Match3ReturnButton,
        RewardTextMatch3,
        Match3WinCoinsParticles,
        Match3ScoreGoalText
    }
}