using Core;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class CookingManager : HOGLogicMonoBehaviour
    {
        [SerializeField] FoodManager foodManager;
        [SerializeField] HOGMoneyHolder playerMoney;

        public const int BAKER_TIME_MULTIPLIER = 2;

        private FoodData foodData;

        public void CookFood(int foodIndex)
        {
            foodData = foodManager.GetFoodData(foodIndex);
            
            if (foodManager.IsFoodOnCooldown(foodIndex))
            {
                return;
            }

            float cookingTime = foodData.CookingTime;
            int profit = foodData.Profit;
            foodManager.SetFoodOnCooldown(foodIndex, true);

            InvokeEvent(HOGEventNames.OnCookFood, foodIndex); // starts the loading bar and timer of cooking

            StartCoroutine(StartCooking(cookingTime, profit, foodIndex));
        }

        public void AutoCookFood(int foodIndex)
        {
            foodData = foodManager.GetFoodData(foodIndex);

            if (foodManager.IsAutoFoodOnCooldown(foodIndex))
            {
                return;
            }

            float cookingTime = foodData.CookingTime * BAKER_TIME_MULTIPLIER;
            int profit = foodData.Profit;
            foodManager.SetAutoFoodOnCooldown(foodIndex, true);

            InvokeEvent(HOGEventNames.OnAutoCookFood, foodIndex); // starts the loading bar and timer of cooking

            StartCoroutine(StartAutoCooking(cookingTime, profit, foodIndex));
        }

        private IEnumerator StartCooking(float cookingTime, int profit, int foodIndex)
        {
            yield return new WaitForSeconds(cookingTime);

            playerMoney.UpdateCurrency(profit);
            foodManager.SetFoodOnCooldown(foodIndex, false);

            InvokeEvent(HOGEventNames.MoneyToastOnCook, foodIndex);
        }

        private IEnumerator StartAutoCooking(float cookingTime, int profit, int foodIndex)
        {
            int index = foodIndex; // created a local copy of the foodIndex variable
            foodData = foodManager.GetFoodData(index);
            yield return new WaitForSeconds(cookingTime);

            playerMoney.UpdateCurrency(profit * foodData.CookFoodTimes);
            foodManager.SetAutoFoodOnCooldown(index, false);

            AutoCookFood(index);
            
            InvokeEvent(HOGEventNames.MoneyToastOnAutoCook, index);
        }

    }
}