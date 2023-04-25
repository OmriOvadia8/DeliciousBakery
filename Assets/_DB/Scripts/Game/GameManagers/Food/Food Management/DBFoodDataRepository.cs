using System.Linq;
using DB_Core;

namespace DB_Game
{
    public class DBFoodDataRepository : IFoodDataRepository
    {
        private FoodDataCollection foodSavedData;

        public FoodDataCollection Foods => foodSavedData;

        public DBFoodDataRepository(FoodDataCollection foods) => foodSavedData = foods;

        public bool IsFoodDataLoaded() => foodSavedData != null;

        public void SaveFoodData() => DBManager.Instance.SaveManager.Save(foodSavedData);

        public FoodData GetFoodData(int foodID)
        {
            var foodData = Foods?.Foods.FirstOrDefault(fd => fd.Index == foodID);

            if (foodData == null)
            {
                DBDebug.LogException("Invalid food ID: " + foodID);
            }

            return foodData;
        }
    }
}