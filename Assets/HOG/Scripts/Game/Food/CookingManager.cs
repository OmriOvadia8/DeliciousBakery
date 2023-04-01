using Core;
using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class CookingManager : HOGLogicMonoBehaviour
    {
        [SerializeField] FoodManager foodManager;
        [SerializeField] HOGMoneyHolder playerMoney;

        private FoodData foodData;

        public void CookFood(int foodIndex) // Active cooking by clicking
        {
            foodData = foodManager.GetFoodData(foodIndex);

            if (foodData.IsOnCooldown)
            {
                return;
            }

            foodData.IsOnCooldown = true;

            HOGManager.Instance.SaveManager.Save(foodManager.foods);

            InvokeEvent(HOGEventNames.OnCookFood, foodIndex); // starts the loading bar and timer of cooking
        }

        public void AutoCookFood(int foodIndex) // Cooking automatically courotine loop after baker unlocked
        {
            foodData = foodManager.GetFoodData(foodIndex);

            if (foodData.IsAutoOnCooldown)
            {
                return;
            }
            foodData.IsAutoOnCooldown = true;

            InvokeEvent(HOGEventNames.OnAutoCookFood, foodIndex); // starts the loading bar and timer of cooking
        }
    }
}