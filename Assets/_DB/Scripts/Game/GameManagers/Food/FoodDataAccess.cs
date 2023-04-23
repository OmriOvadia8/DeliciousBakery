using DB_Core;

namespace DB_Game
{
    public class FoodDataAccess : DBLogicMonoBehaviour
    {
        protected FoodDataRepository foodDataRepository;

        protected virtual void Awake()
        {
            foodDataRepository = new FoodDataRepository(DBFoodManager.Foods);
            DBDebug.Log($"FoodDataAccess: foodDataRepository initialized: {foodDataRepository != null}");
        }
    }
}