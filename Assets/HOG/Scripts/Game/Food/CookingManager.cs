using Core;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;


namespace Game
{
    public class CookingManager : HOGLogicMonoBehaviour
    {
        private FoodManager foodManager;

        void Start()
        {
            foodManager = FindObjectOfType<FoodManager>();
        }

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

            GameLogic.PlayerMoney.UpdateCurrency(profit);

            foodManager.SetFoodOnCooldown(foodIndex, false);
        }


    }
}