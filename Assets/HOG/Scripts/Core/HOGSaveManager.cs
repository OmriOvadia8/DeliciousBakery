using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;


namespace Core
{
    public class HOGSaveManager
    {
        public void Save(IHOGSaveData saveData)
        {
            var saveID = saveData.GetType().FullName;
            HOGDebug.Log(saveID);
            var saveJson = JsonConvert.SerializeObject(saveData);
            HOGDebug.Log(saveJson);

            var path = $"{Application.persistentDataPath}/{saveID}.hogSave";

            File.WriteAllText(path, saveJson);
        }

        public void Load<T>(Action<T> onComplete) where T : IHOGSaveData
        {
            if (!HasData<T>())
            {
                onComplete.Invoke(default);
                return;
            }

            var saveID = typeof(T).FullName;
            var path = $"{Application.persistentDataPath}/{saveID}.hogSave";

            var saveJson = File.ReadAllText(path);
            var saveData = JsonConvert.DeserializeObject<T>(saveJson);

            HOGDebug.Log(saveID);
            HOGDebug.Log(saveJson);

            onComplete.Invoke(saveData);

        }

        public bool HasData<T>() where T : IHOGSaveData
        {
            var saveID = typeof(T).FullName;
            var path = $"{Application.persistentDataPath}/{saveID}.hogSave";
            return File.Exists(path);
        }
    }

    public interface IHOGSaveData
    {
    }
}