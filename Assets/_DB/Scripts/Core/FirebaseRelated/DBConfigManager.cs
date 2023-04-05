using System;
using System.Collections.Generic;
using System.IO;
using Firebase.RemoteConfig;
using Newtonsoft.Json;
using Firebase.Extensions;
using System.Threading.Tasks;

namespace DB_Core
{
    public class DBConfigManager
    {
        private Action onInit;

        public DBConfigManager(Action onComplete)
        {
            DBDebug.Log($"DBConfigManager");

            onInit = onComplete;

            var defaults = new Dictionary<string, object>();
            defaults.Add("upgrade_config", "{}");

            DBDebug.Log("DBConfigManager");
            FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults).ContinueWithOnMainThread(OnDefaultValuesSet);
        }

        private void OnDefaultValuesSet(Task task)
        {
            DBDebug.Log("OnDefaultValuesSet");

            FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero).ContinueWithOnMainThread(OnFetchComplete);
        }

        private void OnFetchComplete(Task obj)
        {
            DBDebug.Log("OnFetchComplete");

            FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(task => OnActivateComplete(task));
        }

        private void OnActivateComplete(Task obj)
        {
            DBDebug.Log("OnActivateComplete");
            onInit.Invoke();
        }

        public void GetConfigAsync<T>(string configID, Action<T> onComplete)
        {
            DBDebug.Log($"GetConfigAsync {configID}");

            var saveJson = FirebaseRemoteConfig.DefaultInstance.GetValue(configID).StringValue;

            var saveData = JsonConvert.DeserializeObject<T>(saveJson);

            onComplete.Invoke(saveData);
        }

        public void GetConfigOfflineAsync<T>(string configID, Action<T> onComplete)
        {
            var path = $"Assets/_DB/Config/{configID}.json";

            var saveJson = File.ReadAllText(path);
            var saveData = JsonConvert.DeserializeObject<T>(saveJson);

            onComplete.Invoke(saveData);
        }
    }
}