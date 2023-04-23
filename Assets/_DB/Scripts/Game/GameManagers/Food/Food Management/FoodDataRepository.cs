using UnityEngine;
using System.Linq;
using DB_Core;

namespace DB_Game
{
    public class FoodDataRepository : IFoodDataRepository
    {
        private FoodDataCollection foodSavedData;

        public FoodDataCollection Foods => foodSavedData;

        public FoodDataRepository(FoodDataCollection foods)
        {
            foodSavedData = foods;
        }

        public FoodData GetFoodData(int foodID)
        {
            var foodData = Foods?.Foods.FirstOrDefault(fd => fd.Index == foodID);

            if (foodData == null)
            {
                Debug.LogError("Invalid food ID: " + foodID);
            }

            return foodData;
        }

        public bool IsFoodDataLoaded()
        {
            return foodSavedData != null;
        }

        public void SaveFoodData()
        {
            DBManager.Instance.SaveManager.Save(foodSavedData);
        }
    }
}