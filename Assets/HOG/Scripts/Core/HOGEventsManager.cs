using System;
using System.Collections.Generic;

namespace Core
{
    public class HOGEventsManager
    {
        private Dictionary<string, List<HOGEvent>> activeListeners = new();

        public void AddListener(HOGEvent actualEvent)
        {
            if (activeListeners.TryGetValue(actualEvent.eventName, out var listOfEvents))
            {
                listOfEvents.Add(actualEvent);
                return;
            }

            activeListeners.Add(actualEvent.eventName, new List<HOGEvent> { actualEvent });
        }

        public void RemoveListener(HOGEvent actualEvent)
        {
            if (activeListeners.TryGetValue(actualEvent.eventName, out var listOfEvents))
            {
                listOfEvents.Remove(actualEvent);

                if (listOfEvents.Count <= 0)
                {
                    activeListeners.Remove(actualEvent.eventName);
                }
            }
        }

        public void InvokeEvent(string eventName, object obj)
        {
            if (activeListeners.TryGetValue(eventName, out var listOfEvents))
            {
                foreach (var hogEvent in listOfEvents)
                {
                    hogEvent.eventAction.Invoke(obj);
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