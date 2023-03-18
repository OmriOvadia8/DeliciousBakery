using System;
using System.Collections.Generic;

namespace Core
{
    public class HOGEventsManager
    {
        private readonly Dictionary<HOGEventNames, List<Action<object>>> activeListeners = new();

        public void AddListener(HOGEventNames eventName, Action<object> onGameStart)
        {
            if (activeListeners.TryGetValue(eventName, out var listOfEvents))
            {
                listOfEvents.Add(onGameStart);
                return;
            }

            activeListeners.Add(eventName, new List<Action<object>> { onGameStart });
        }

        public void RemoveListener(HOGEventNames eventName, Action<object> onGameStart)
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

        public void InvokeEvent(HOGEventNames eventName, object obj)
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

    public enum HOGEventNames
    {
        OnCurrencySet,
        OnGameStart,
        OnUpgraded,
        OnCookFood,
        MoneyToastOnCook,
        MoneyToastOnAutoCook,
        OnUpgradeMoneySpentToast,
        OnHireMoneySpentToast,
        OnHired,
        OnAutoCookFood,
        OnPause,
        OfflineTimeRefreshed,
        OnPopupOpen,
        OnPopupClose
    }
}