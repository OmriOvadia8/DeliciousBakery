using System;
using System.Collections.Generic;

namespace Core
{
    public class HOGEventsManager
    {
        private Dictionary<string, List<Action<object>>> activeListeners = new();

        public void AddListener(string eventName, Action<object> listener)
        {
            if (activeListeners.TryGetValue(eventName, out var listOfEvents))
            {
                listOfEvents.Add(listener);
                return;
            }

            activeListeners.Add(eventName, new List<Action<object>> { listener});
        }

        public void RemoveListener(string eventName, Action<object> onGameStart)
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

        public void InvokeEvent(string eventName, object obj = null)
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

    public class HOGEvent
    {
        public string eventName;
        public Action<object> eventAction;

    }
}