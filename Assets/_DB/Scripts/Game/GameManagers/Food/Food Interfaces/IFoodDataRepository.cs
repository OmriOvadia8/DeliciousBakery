namespace DB_Game
{
    public interface IFoodDataRepository
    {
        FoodData GetFoodData(int foodID);
        void SaveFoodData();
        bool IsFoodDataLoaded();
    }
}
