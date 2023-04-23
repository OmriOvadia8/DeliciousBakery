using DB_Game;
using UnityEngine;

public class FoodDataAccess : DBLogicMonoBehaviour
{
    protected FoodDataRepository foodDataRepository;

    protected virtual void Awake()
    {
        foodDataRepository = new FoodDataRepository(DBFoodManager.Foods);
    }
}
