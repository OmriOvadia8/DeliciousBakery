using Core;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;


namespace Game
{
    public class CookingManager : HOGMonoBehaviour
    {
        private HOGMoneyHolder Currency => HOGGameLogic.Instance.PlayerMoney;


       // private bool[] isFoodOnCooldown;
 
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

            float cookingTime = foodData.cookingTime;
            int profit = foodData.profit * foodData.level;
            Debug.Log(foodData.level);
            Debug.Log(foodData.levelUpCost);
            foodManager.SetFoodOnCooldown(foodIndex, true);
            StartCoroutine(StartCooking(cookingTime, profit, foodIndex));
        }

        private IEnumerator StartCooking(float cookingTime, int profit, int foodIndex)
        {
            yield return new WaitForSeconds(cookingTime);

            Currency.UpdateCurrency(profit);

            foodManager.SetFoodOnCooldown(foodIndex, false);
        }


    }
}