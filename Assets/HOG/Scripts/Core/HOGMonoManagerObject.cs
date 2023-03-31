using System;
using UnityEditor;

namespace Core
{
    public class HOGMonoManagerObject : HOGMonoBehaviour
    {
        private void OnApplicationPause(bool pauseStatus)
        {
            Manager.EventsManager.InvokeEvent(HOGEventNames.OnPause, pauseStatus);
        }
    }
}