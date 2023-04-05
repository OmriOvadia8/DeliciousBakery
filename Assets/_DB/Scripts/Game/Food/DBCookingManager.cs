using DB_Core;
using UnityEngine;

namespace DB_Game
{
    public class DBCookingManager : DBLogicMonoBehaviour
    {
        [SerializeField] DBCurrencyManager currencyManager;

        private FoodData foodData;

        public void CookFood(int foodIndex) // Active cooking by clicking
        {
            foodData = DBFoodManager.GetFoodData(foodIndex);

            if (foodData.IsOnCooldown)
            {
                return;
            }

            foodData.IsOnCooldown = true;

            DBManager.Instance.SaveManager.Save(DBFoodManager.foods);

            InvokeEvent(DBEventNames.OnCookFood, foodIndex); // starts the loading bar and timer of cooking
        }

        public void AutoCookFood(int foodIndex) // Cooking automatically courotine loop after baker unlocked
        {
            foodData = DBFoodManager.GetFoodData(foodIndex);

            if (foodData.IsAutoOnCooldown)
            {
                return;
            }
            foodData.IsAutoOnCooldown = true;

            InvokeEvent(DBEventNames.OnAutoCookFood, foodIndex); // starts the loading bar and timer of cooking
        }
    }
}