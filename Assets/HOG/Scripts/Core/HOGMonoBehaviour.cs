using System;
using UnityEngine;

namespace Core
{
    public class HOGMonoBehaviour : MonoBehaviour
    {
        protected HOGManager Manager => HOGManager.Instance;

        protected void AddListener(HOGEventNames eventName, Action<object> onGameStart) => Manager.EventsManager.AddListener(eventName, onGameStart);
        protected void RemoveListener(HOGEventNames eventName, Action<object> onGameStart) => Manager.EventsManager.RemoveListener(eventName, onGameStart);
        protected void InvokeEvent(HOGEventNames eventName, object obj) => Manager.EventsManager.InvokeEvent(eventName, obj);
    }
}