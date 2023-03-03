using System;
using System.Collections.Generic;
using System.IO;
using Firebase.RemoteConfig;
using Newtonsoft.Json;
using Firebase.Extensions;
using System.Threading.Tasks;

namespace Core
{
    public class HOGConfigManager
    {
        private Action onInit;

        public HOGConfigManager(Action onComplete)
        {
            HOGDebug.Log($"HOGConfigManager");

            onInit = onComplete;

            var defaults = new Dictionary<string, object>();
            defaults.Add("upgrade_config", "{}");

            HOGDebug.Log("HOGConfigManager");
            FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults).ContinueWithOnMainThread(OnDefaultValuesSet);
        }

        private void OnDefaultValuesSet(Task task)
        {
            HOGDebug.Log("OnDefaultValuesSet");

            FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero).ContinueWithOnMainThread(OnFetchComplete);
        }

        private void OnFetchComplete(Task obj)
        {
            HOGDebug.Log("OnFetchComplete");

            FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(task => OnActivateComplete(task));
        }

        private void OnActivateComplete(Task obj)
        {
            HOGDebug.Log("OnActivateComplete");
            onInit.Invoke();
        }

        public void GetConfigAsync<T>(string configID, Action<T> onComplete)
        {
            HOGDebug.Log($"GetConfigAsync {configID}");

            var saveJson = FirebaseRemoteConfig.DefaultInstance.GetValue(configID).StringValue;

            var saveData = JsonConvert.DeserializeObject<T>(saveJson);

            onComplete.Invoke(saveData);
        }

        public void GetConfigOfflineAsync<T>(string configID, Action<T> onComplete)
        {
            var path = $"Assets/_HOG/Config/{configID}.json";

            var saveJson = File.ReadAllText(path);
            var saveData = JsonConvert.DeserializeObject<T>(saveJson);

            onComplete.Invoke(saveData);
        }
    }
}