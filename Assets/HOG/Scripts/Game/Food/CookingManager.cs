using Core;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;


namespace Game
{
    public class CookingManager : HOGLogicMonoBehaviour
    {
        [SerializeField] FoodManager foodManager;
        [SerializeField] HOGMoneyHolder playerMoney;

        public void CookFood(int foodIndex)
        {
            FoodData foodData = foodManager.GetFoodData(foodIndex);
            if (foodManager.IsFoodOnCooldown(foodIndex))
            {
                return;
            }

            float cookingTime = foodData.CookingTime;
            int profit = foodData.Profit;
            foodManager.SetFoodOnCooldown(foodIndex, true);
            StartCoroutine(StartCooking(cookingTime, profit, foodIndex));
        }

        private IEnumerator StartCooking(float cookingTime, int profit, int foodIndex)
        {
            yield return new WaitForSeconds(cookingTime);

            playerMoney.UpdateCurrency(profit, foodManager.GetFoodData(foodIndex).Level);
                        
            foodManager.SetFoodOnCooldown(foodIndex, false);
        }
    }
}