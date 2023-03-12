using Core;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class CookingManager : HOGLogicMonoBehaviour
    {
        [SerializeField] FoodManager foodManager;
        [SerializeField] HOGMoneyHolder playerMoney;
        [SerializeField] UIManager uiManager;

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

        private IEnumerator StartCooking(float cookingTime, int profit, int foodIndex)
        {
            yield return new WaitForSeconds(cookingTime);

            playerMoney.UpdateCurrency(profit);
            foodManager.SetFoodOnCooldown(foodIndex, false);

            if(foodData.IsIdleFood == true)
            {
                foodManager.UnlockIdleFood(foodIndex);
            }

            InvokeEvent(HOGEventNames.MoneyToastOnCook, foodIndex);
            uiManager.UpgradeButtonsCheck();
        }
    }
}