using DB_Core;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace DB_Game
{
    public class FoodDataManager : IDBSaveData
    {
        public Dictionary<int, FoodData> Foods { get; set; }

        public void LoadFoods(Action<Dictionary<int, FoodData>> onComplete)
        {
            DBManager.Instance.SaveManager.Load<FoodDataManager>((foodSaveData) =>
            {
                if (foodSaveData != null)
                {
                    Foods = foodSaveData.Foods;
                    onComplete.Invoke(Foods);
                }
                else
                {
                    onComplete.Invoke(null);
                }
            });
        }
        public void SaveFoods()
        {
            DBManager.Instance.SaveManager.Save(this);
            // Serialize food data to JSON
            var json = JsonConvert.SerializeObject(Foods);

            // Debug log saved JSON data
            DBDebug.Log(json);

            // Save JSON data to file
            File.WriteAllText("food_data", json);
        }
    }
}