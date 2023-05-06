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
                //TODO: Do For Loop
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
        OnGameStart,
        OnUpgradeTextUpdate,
        OnCookFood,
        MoneyToastOnCook,
        MoneyToastOnAutoCook,
        OnUpgradeMoneySpentToast,
        OnHireMoneySpentToast,
        OnHiredTextUpdate,
        OnAutoCookFood,
        OnPause,
        OfflineTimeRefreshed,
        OnPopupOpen,
        OnPopupClose,
        OnLearnRecipe,
        OnLearnRecipeSpentToast,
        MoneyToastOnDoubleReward,
        OnAutoCookOnResume,
        OnAutoCookAfterPause,
        CookFoodAfterOffline,
        Letstrythisout,
        CurrencyUpdateUI,
        OnCompleteCookFood,
        OnPremCurrencySet,
        PremCurrencyUpdateUI,
        DeviceAppearAnimation,
        CheckDeviceAppearance,
        AddCurrencyUpdate,
        StartActiveCooking,
        StartBakerCooking,
        BuyButtonsCheck,
        CookButtonAlphaOn,
        CookButtonAlphaOff,
        OnUnFocused,
        BakerParticles,
        LearnParticles,
        FoodBarReveal,
        FoodBarLocked,
        UpgradeParticles,
        CookParticles,
        CookFoodButtonCheck,
        SaveAchievements,
        CheckCookedAchievement,
        CheckHiredAchievement,
        CheckTotalHiredAchievement,
        CheckTotalCookedAchievement,
        CurrentMakeFoodAchievementStatus,
        CurrentHireBakerAchievementStatus,
        AddStarsUpdate
    }
}