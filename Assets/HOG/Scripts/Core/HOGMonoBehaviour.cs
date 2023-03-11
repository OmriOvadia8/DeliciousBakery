using System;
using System.Collections;
using UnityEngine;

namespace Core
{
    public class HOGMonoBehaviour : MonoBehaviour
    {
        protected HOGManager Manager => HOGManager.Instance;

        protected void AddListener(HOGEventNames eventName, Action<object> onGameStart) => Manager.EventsManager.AddListener(eventName, onGameStart);
        protected void RemoveListener(HOGEventNames eventName, Action<object> onGameStart) => Manager.EventsManager.RemoveListener(eventName, onGameStart);
        protected void InvokeEvent(HOGEventNames eventName, object obj) => Manager.EventsManager.InvokeEvent(eventName, obj);


        public Coroutine WaitForSeconds(float time, Action onComplete)
        {
            return StartCoroutine(WaitForSecondsCoroutine(time, onComplete));
        }

        private IEnumerator WaitForSecondsCoroutine(float time, Action onComplete)
        {
            yield return new WaitForSeconds(time);
            onComplete?.Invoke();
        }

        public Coroutine WaitForFrame(Action onComplete)
        {
            return StartCoroutine(WaitForFrameCoroutine(onComplete));
        }

        private IEnumerator WaitForFrameCoroutine(Action onComplete)
        {
            yield return null;
            onComplete?.Invoke();
        }

    }
}