using System;
using UnityEngine;

namespace Core
{
    public class HOGMonoBehaviour : MonoBehaviour
    {
        protected HOGManager Manager => HOGManager.Instance;

        protected void AddListener(HOGEvent hogEvent) => Manager.EventsManager.AddListener(hogEvent);
        protected void RemoveListener(HOGEvent hogEvent) => Manager.EventsManager.RemoveListener(hogEvent);
        protected void InvokeEvent(string eventName, object obj) => Manager.EventsManager.InvokeEvent(eventName, obj);
    }
}