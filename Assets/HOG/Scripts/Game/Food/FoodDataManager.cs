using Core;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace Game
{
    public class FoodDataManager : IHOGSaveData
    {
        public Dictionary<int, FoodData> Foods { get; set; }

        // Load the JSON data into the foods dictionary instance
        public void LoadFoods(Action<Dictionary<int, FoodData>> onComplete)
        {
            HOGManager.Instance.SaveManager.Load<FoodDataManager>((foodSaveData) =>
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
            HOGManager.Instance.SaveManager.Save(this);
            // Serialize food data to JSON
            var json = JsonConvert.SerializeObject(Foods);

            // Debug log saved JSON data
            Debug.Log(json);

            // Save JSON data to file
            File.WriteAllText("food_data", json);
        }
    }
}