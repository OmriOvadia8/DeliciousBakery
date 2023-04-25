namespace DB_Game
{
    public class FoodDataAccess : DBLogicMonoBehaviour
    {
        protected DBFoodDataRepository foodDataRepository;

        protected virtual void Awake() => foodDataRepository = new DBFoodDataRepository(DBFoodManager.Foods);

        protected FoodData GetFoodData(int foodID) => foodDataRepository.GetFoodData(foodID);

        protected void SaveFoodData() => foodDataRepository.SaveFoodData();
    }
}