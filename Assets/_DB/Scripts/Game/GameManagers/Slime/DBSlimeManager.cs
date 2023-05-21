using System.Collections.Generic;
using UnityEngine;
using DB_Core;

namespace DB_Game
{
    public class DBSlimeManager : FoodDataAccess
    {
        [SerializeField] GameObject[] chefSlimes;
        private Dictionary<int, int> indexToSlimeMap;
    
        private void OnEnable() => AddListener(DBEventNames.SlimeAction, OnNewDeviceActivated);
    
        private void OnDisable() => RemoveListener(DBEventNames.SlimeAction, OnNewDeviceActivated);
    
        private void Start()
        {
            SlimeDictionarySetUp();
    
            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                ActivateSlimeIfUnlocked(i);
            }
        }
    
        private void OnNewDeviceActivated(object index) => ActivateSlimeIfUnlocked((int)index);
    
        private void ActivateSlimeIfUnlocked(int index)
        {
            if (indexToSlimeMap.ContainsKey(index))
            {
                var foodData = GetFoodData(index);
                if (foodData.IsBakerUnlocked)
                {
                    int slimeIndex = indexToSlimeMap[index];
                    chefSlimes[slimeIndex].SetActive(true);
                }
            }
        }
    
        private void SlimeDictionarySetUp()
        {
            indexToSlimeMap = new()
        {
            { 0, 0 },
            { 1, 1 },
            { 2, 1 },
            { 3, 2 },
            { 5, 2 },
            { 6, 3 },
            { 7, 3 },
        };
        }
    }
}
