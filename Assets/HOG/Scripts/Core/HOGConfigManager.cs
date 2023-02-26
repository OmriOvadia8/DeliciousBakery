using System;
using System.IO;
using Newtonsoft.Json;

namespace Core
{
    public class HOGConfigManager
    {
        public void GetConfigAsync<T>(string configID, Action<T> onComplete)
        {
            var path = $"Assets/HOG/Config/{configID}.json";

            var saveJson = File.ReadAllText(path);
            var saveData = JsonConvert.DeserializeObject<T>(saveJson);

            onComplete.Invoke(saveData);
        }
    }
}